using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class MusicJson
{
    public string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public NotesJson[] notes;
}

[System.Serializable]
public class NotesJson
{
    public int LPB;
    public int num;
    public int block;
    public int type;
    public NotesJson[] notes;
}

public class PlaySceneProcessManager : MonoBehaviour
{
    static List<List<NoteData>> _notes = new List<List<NoteData>>(); // 2次元リスト
    static float laneWidth = 0.3f; //レーンの太さ( = ノーツの太さ )
    float _offset = 3f * NotesFallUpdater.speed;
    public static bool isPose { get; private set; } = true;
    static float musicTime;
    [SerializeField] LongNotesGenerator lng;
    [SerializeField] AudioSource bgm;
    [SerializeField] static AudioSource metro;
    [SerializeField] static AudioSource success;
    [SerializeField] static AudioSource frick;
    void Start()
    {
        AudioSource[] AS = GetComponents<AudioSource>();
        metro = AS[1];
        success = AS[2];
        frick = AS[3];
        TextAsset jsonFile = Resources.Load("UFOCATCHER9_BGM") as TextAsset;
        string inputString = jsonFile.ToString();
        MusicJson music = JsonUtility.FromJson<MusicJson>(inputString);
        MusicData.BPM = music.BPM;
        MusicData.musicName = music.name;
        //Debug.Log(MusicData.BPM);

        //Debug.Log(music);
        musicTime = -3.0f;
        isPose = false;
        LoadNotes(music);
        //Invoke("NotesStart", 1);
        //InvokeRepeating("Metro", 1, 60f / MusicData.BPM);
        Invoke("BGMStart", 3); // ノーツ再生から3秒待たなければならない
    }

    void Update()
    {
        //Debug.Log(musicTime);
        if (!isPose)
        {
            musicTime += Time.deltaTime;
        }
    }

    static void Metro()
    {
        metro.Stop();
        metro.PlayOneShot(metro.clip);
    }

    static void Success(int type)
    {
        metro.Stop();
        //success.Stop();
        if (type != 5) success.PlayOneShot(success.clip);
        else frick.PlayOneShot(frick.clip);
    }

    static void Bad()
    {
        //metro.Stop();
        metro.PlayOneShot(metro.clip);
    }

    void LoadNotes(MusicJson music)
    {
        Object noteObject = Resources.Load("Notes");
        Object noteObjectF = Resources.Load("FrickNotes");

        for (int i = 0; i < music.maxBlock; i++)
        {
            _notes.Add(new List<NoteData>());
        }

        for (int i = 0; i < music.notes.Length; i++)
        {
            NoteData firstNote = new NoteData();
            NoteData nextNote = new NoteData();
            firstNote.LPB = music.notes[i].LPB;
            firstNote.num = music.notes[i].num;
            firstNote.block = music.notes[i].block;
            firstNote.type = music.notes[i].type;
            //Debug.Log(firstNote.num);
            float a = 60f * firstNote.num;
            float b = MusicData.BPM * firstNote.LPB;
            firstNote.timing = a / b;
            //Debug.Log(i + " " + a);
            //Debug.Log(i + " " + b);
            //Debug.Log(i + " " + firstNote.block + " " + firstNote.timing);
            firstNote.noteObjects = new List<GameObject>();
            _notes[firstNote.block].Add(firstNote);
            if (music.notes[i].notes.Length == 1)
            {
                nextNote.LPB = music.notes[i].notes[0].LPB;
                nextNote.num = music.notes[i].notes[0].num;
                nextNote.block = music.notes[i].notes[0].block;
                nextNote.type = music.notes[i].notes[0].type;
                a = 60f * nextNote.num;
                b = MusicData.BPM * nextNote.LPB;
                nextNote.timing = a / b;
                _notes[nextNote.block].Add(nextNote);
            }

            if (firstNote.type == 1) // 生成は別のところで
            {
                firstNote.noteObjects.Add((GameObject)Instantiate(noteObject, new Vector3(-0.9f + laneWidth * firstNote.block, NotesFallUpdater.speed * firstNote.timing + _offset, -0.005f), new Quaternion(0, 0, 0, 0)));
            }
            else if (firstNote.type == 2)
            {
                firstNote.noteObjects = lng.Create(firstNote.block, nextNote.block, firstNote.timing, nextNote.timing);
            }
            else if (firstNote.type == 5)
            {
                firstNote.noteObjects.Add((GameObject)Instantiate(noteObjectF, new Vector3(-0.9f + laneWidth * firstNote.block, NotesFallUpdater.speed * firstNote.timing + _offset, -0.005f), new Quaternion(0, 0, 0, 0)));
            }
        }

        for (int i = 0; i < _notes.Count; i++)
            _notes[i].OrderBy(item => item.timing);

        //Debug.Log(_notes[0][0].block + _notes[0][0].timing);
        //Debug.Log(_notes[0][1].block + _notes[0][1].timing);
    }

    void BGMStart()
    {
        bgm.Play();
        //isPose = false;
    }

    public static void JudgeTiming(int lineNum, int type)
    {
        NoteData note;
        note = _notes[lineNum].Find(n => Mathf.Abs(n.timing - musicTime) <= 0.5f && n.type == type);
        if (note != null)
        {
            //Debug.Log("OK!: " + musicTime + " " + lineNum);
            Success(note.type);
            if (note.noteObjects != null && note.noteObjects[0] != null && note.type != 2) Destroy(note.noteObjects[0]);
            _notes[lineNum].Remove(note);
            return;
        }
        else
        {
            //Debug.Log("Failed.: " + musicTime + " " + lineNum);
            //Bad();
        }

        if (type == 5)
            JudgeTiming(lineNum, 2);

        if (type == 1)
            JudgeTiming(lineNum, 2);
        // 5のときは判定して見つからなければ2を考える
    }
}
