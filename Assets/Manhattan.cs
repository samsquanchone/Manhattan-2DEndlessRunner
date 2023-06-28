using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Manhattan : MonoBehaviour {
    public interface Listener {
        public abstract void OnInput(string message);     // handles messages from Manhattan
    }

    public class Musician : MonoBehaviour {
        // Instrument/channel mapping
        public string instrument = null;                // instrument filter (null = all instruments)
        public uint channel = 0;                        // channel filter (0 = all channels)
        protected bool active = true;
        public void SetActive(bool enabled) { active = enabled; } // used to enable/disable musician

        public float tempo = 80.0f;
        public Vector2 timeSignature = new Vector2(4, 4);

        // Status - maintains the running status of currently playing notes
        protected class Status {
            public Status() {
                note = new Manhattan.Note();
                pitches = new bool[128];
                for (int p = 0; p < 128; p++) { pitches[p] = false; }
                notes = 0;
            }
            public Manhattan.Note note; // current/last note
            public bool[] pitches;      // active status (on/off) of all pitches
            public int notes;           // total number of playing notes

            // Updates status upon the start of a new note
            public void NoteOn(Manhattan.Note note) {
                this.note = note;
                if (note.pitch < 128 && !pitches[note.pitch]) {
                    pitches[note.pitch] = true;
                    notes++;
                }
            }

            // Updates status upon the end of a note
            public void NoteOff(Manhattan.Note note) {
                this.note = note;
                if (note.pitch < 128 && pitches[note.pitch]) {
                    pitches[note.pitch] = false;
                    notes--;
                }
            }
        }
        protected Status status = new Status();
        
        protected void Awake() {

        }

        protected void LateUpdate() {

        }

        bool listening = true;
        public void Reset() {
            listening = true;
        }

        public void NoteOn(Manhattan.Note note) {
            status.NoteOn(note);
            OnNoteOn(note);
        }

        public void NoteOff(Manhattan.Note note) {
            status.NoteOff(note);
            OnNoteOff(note);
        }

        // Called when a "beat" message is received
        virtual public void OnBeat() { }

        // Called when a "note +" message is received
        protected virtual void OnNoteOn(Manhattan.Note note) { }

        // Called when a "note -" message is received
        protected virtual void OnNoteOff(Manhattan.Note note) { }

        // Returns the list of musicians associated with a particular note, given a list of all musicians and specified filter.
        public static List<Musician> Filter(List<Musician> musicians, Manhattan.Note note) {
            List<Musician> filtered = new List<Musician>();
            foreach (Musician m in musicians) {
                if ((m.instrument == null || m.instrument == "" || m.instrument == note.instrument) && (m.channel == 0 || m.channel == note.channel))
                    filtered.Add(m);
            }
            return filtered;
        }
    }

    public struct Note {
        public bool on;
        public string instrument;
        public int pitch;
        public int channel;

        public bool isDrum() {
            const string kits = "RMIPJHOW";
            return instrument[0] == 'D' && kits.Contains(instrument[1]);
        }

        static readonly string[] strings = { "S1", "S2", "S3", "ST", "Vi", "Va", "Vc", "Cb" };
        public bool isBowed() {
            foreach (string s in strings) {
                if (instrument == s)
                    return true;
            }
            return false;
        }

        public bool isVocal() {
            return instrument == "Ah" || instrument == "Do";
        }
    };

    // Status - maintains the running status of currently playing notes (instrument agnostic)
    class Status {
        public Status() {
            note = new Manhattan.Note();
            pitches = new bool[128];
            for (int p = 0; p < 128; p++) { pitches[p] = false; }
            notes = 0;
        }
        public Manhattan.Note note; // current/last note
        public bool[] pitches;      // active status (on/off) of all pitches
        public int notes;           // total number of playing notes

        // Updates status upon the start of a new note
        public void NoteOn(Manhattan.Note note) {
            this.note = note;
            if (note.pitch < 128 && !pitches[note.pitch]) {
                pitches[note.pitch] = true;
                notes++;
            }
        }

        // Updates status upon the end of a note
        public void NoteOff(Manhattan.Note note) {
            this.note = note;
            if (note.pitch < 128 && pitches[note.pitch]) {
                pitches[note.pitch] = false;
                notes--;
            }
        }
    }
    Status status = new Status();

    [Tooltip("Output Level (gain)\n(default: 1.0)")]
    public float OutputLevel = 1.0F;                  // level (0.0 to 1.0)

    public enum Mode { Embedded, LiveEditing };
    [Tooltip("Embedded Mode\nLoad file and generate audio inside the game.\n\nLive Editing Mode\nConnect and sync with the Manhattan app.")]
    public Mode PlaybackMode = Mode.Embedded;   // live / embedded mode

    [Tooltip("Filename\n(e.g. manhattan.zmu)")]
    public string Filename = "manhattan.zmu";   // filename in StreamingAssets folder (e.g. manhattan.zmu)

    [Tooltip("Automatically Start Playback?")]
    public bool AutoPlay = true;                // automatically play on startup

    [Tooltip("Show Debug Output from Manhattan")]
    public bool DebugOutput = false;            // automatically play on startup

    private float UpdateInterval = 0.1f;        // reserved for future use
    
    public List<GameObject> Listeners = new List<GameObject>();
    private List<Listener> _listeners = new List<Listener>();

    protected float _time = 0.0f;

    //unsafe static Manhattan* Instance = null;

    private bool active = false;
    static IntPtr _manhattan = IntPtr.Zero;
    public List<Musician> Musicians = new List<Musician>();

    // create / connect with a Manhattan instance
    private IntPtr Create(bool isLive/*, Callback callback = null*/) {
        if (active && _manhattan != null) return _manhattan;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<IntPtr, ManhattanCreate>(_library, PlaybackMode == Mode.LiveEditing);
#else
        return ManhattanCreate(PlaybackMode == Mode.LiveEditing);
#endif
    }

    // check Manhattan library functionality
    private bool Check() {
        //if (!active) return false;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanCheck>(_library, 2, -1) == 1;
#else
        return ManhattanCheck(2, -1) == 1;
#endif
    }

    // check communication pipe to Manhattan
    private int Test() {
        if (!active) return 1;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanTest>(_library, _manhattan);
#else
        return ManhattanTest(_manhattan);
#endif
    }

    // check communication pipe to Manhattan
    unsafe public int Data(string destination, byte[] data) {
        return Data(destination, data, data.Length);
    }
    unsafe public int Data(string destination, byte[] data, int count) {
        if (!active) return 1;

        IntPtr buffer;
        fixed(byte* ptr = data) { buffer = (IntPtr)ptr; }
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanData>(_library, _manhattan, destination, buffer, count);
#else
        return ManhattanData(_manhattan, destination, buffer, count);
#endif
    }

    // handle messages returned from Manhattan
    List<string> _messages = new List<string>(); // buffer messages from audio thread
    protected void Input(string message) {
        if (!active) return;

        if (message.StartsWith("Time")) {
            var args = message.Split(' ');
            foreach (Musician m in Musicians) {
                m.tempo = (float)Convert.ToDouble(args[1]);
                m.timeSignature.x = Convert.ToInt32(args[2]);
                m.timeSignature.y = Convert.ToInt32(args[3]);
                m.OnBeat();
            }
        } else if (message.StartsWith("Beat")) {
            foreach (Musician m in Musicians)
                m.OnBeat();
        } else if (message.StartsWith("Note")) {
            string[] args = message.Split(' ');
            Note note = new Note();

            foreach (Musician m in Musicians)
                m.Reset(); // new frame

            for (int n = 1; n < args.Length; n++) {
                note.on = args[n][0] == '+';
                note.instrument = args[n].Substring(1, 2);
                note.pitch = int.Parse(args[n].Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                if (args[n].Length == 7)
                    note.channel = int.Parse(args[n].Substring(5, 2));
                List<Musician> musicians = Musician.Filter(Musicians, note);
                foreach (Musician m in musicians) {
                    if (note.on) m.NoteOn(note);
                    else m.NoteOff(note);
                }
            }
        } else if (_listeners.Count > 0) {
            foreach (Listener l in _listeners)
                l.OnInput(message);
        } else {
            Debug.LogFormat("M: {0}", message);
        }
    }

    // configure the Manhattan's audio output
    private int Config(int samplerate, int channels) {
        if (!active) return 1;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanConfig>(_library, _manhattan, samplerate, channels);
#else
        return ManhattanConfig(_manhattan, samplerate, channels);
#endif
    }

    // generate music and populate supplied audio buffer
    private int Process(IntPtr buffer, int length, int channels, double dspTime) {
        if (!active) return 1;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanProcess>(_library, _manhattan, buffer, length, channels, dspTime);
#else
        return ManhattanProcess(_manhattan, buffer, length, channels, dspTime);
#endif
    }

    // open song file in Manhattan (e.g. manhattan.zmu)
    public int Open(string path) {
        if (!active) return 1;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanOpen>(_library, _manhattan, path);
#else
        return ManhattanOpen(_manhattan, path);
#endif
    }

    // start playback
    public int Play() {
        if (!active) return 1;
#if UNITY_EDITOR //|| !IOS
        return Native.Invoke<int, ManhattanPlay>(_library, _manhattan);
#else
        return ManhattanPlay(_manhattan);
#endif
    }

    // send MIDI bytes to Manhattan
    public void MIDI(byte status, byte data1, byte data2) {
        if (!active) return;
#if UNITY_EDITOR //|| !IOS
        Native.Invoke<ManhattanMIDI>(_library, _manhattan, status, data1, data2);
#else
        ManhattanMIDI(_manhattan, status, data1, data2);
#endif
    }

    // run the code in the Manhattan cell with the specified label
    public void Run(string label) {
        if (!active) return;
#if UNITY_EDITOR //|| !IOS
        Native.Invoke<ManhattanRun>(_library, _manhattan, label);
#else
        ManhattanRun(_manhattan, label);
#endif
    }

    // set the value of the Manhattan cell parameter with the specified label
    public void Set(string label, float value) {
        if (!active) return;
#if UNITY_EDITOR //|| !IOS
        Native.Invoke<ManhattanSet>(_library, _manhattan, label, value);
#else
        ManhattanSet(_manhattan, label, value);
#endif
    }

    // execute the specified Manhattan formula
    public void Code(string formula) {
        if (!active) return;
#if UNITY_EDITOR //|| !IOS
        Native.Invoke<ManhattanCode>(_library, _manhattan, formula);
#else
        ManhattanCode(_manhattan, formula);
#endif
    }

    StringBuilder message;

    // execute the specified Manhattan formula
    unsafe string Receive() {
        if (!active) return null;

        StringBuilder message = new StringBuilder(1024);
#if UNITY_EDITOR //|| !IOS
        if (Native.Invoke<int, ManhattanReceive>(_library, _manhattan, message, message.Capacity) != 0)
#else
        if (ManhattanReceive(_manhattan, message, message.Capacity) != 0)
#endif
            return message.ToString();
        else
            return null;
    }

#if UNITY_EDITOR //|| !IOS
    static IntPtr _library = IntPtr.Zero;

    delegate int ManhattanCheck(int num1, int num2); // TEMP
    delegate int ManhattanTest(IntPtr manhattan);

    delegate IntPtr ManhattanCreate(bool isLive/*, [MarshalAs(UnmanagedType.FunctionPtr)] Callback callback*/);
    delegate void ManhattanDestroy(IntPtr manhattan);

    delegate int ManhattanConfig(IntPtr manhattan, int samplerate, int channels);
    delegate int ManhattanOpen(IntPtr manhattan, string path);
    delegate int ManhattanPlay(IntPtr manhattan);

    delegate int ManhattanMIDI(IntPtr manhattan, byte status, byte data1, byte data2);
    delegate int ManhattanRun(IntPtr manhattan, string formula);
    delegate int ManhattanSet(IntPtr manhattan, string label, float value);
    delegate int ManhattanCode(IntPtr manhattan, string formula);
    delegate int ManhattanData(IntPtr manhattan, string destination, IntPtr data, int length);

    unsafe delegate int ManhattanProcess(IntPtr engine, IntPtr buffer, int length, int channels, double dspTime);
    unsafe delegate int ManhattanReceive(IntPtr manhattan, StringBuilder message, int capacity);
    //unsafe delegate int ManhattanRequest(IntPtr manhattan, string request, StringBuilder reply, int reply_size);
#else
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanCheck(int num1, int num2); // TEMP
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanTest(IntPtr manhattan);

    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr ManhattanCreate(bool isLive/*, [MarshalAs(UnmanagedType.FunctionPtr)] Callback callback*/);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern void ManhattanDestroy(IntPtr manhattan);

    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanConfig(IntPtr manhattan, int samplerate, int channels);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanOpen(IntPtr manhattan, string path);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanPlay(IntPtr manhattan);

    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanMIDI(IntPtr manhattan, byte status, byte data1, byte data2);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanRun(IntPtr manhattan, string formula);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanSet(IntPtr manhattan, string label, float value);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanCode(IntPtr manhattan, string formula);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanData(IntPtr manhattan, string destination, IntPtr data, int length);

    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanProcess(IntPtr engine, IntPtr buffer, int length, int channels, double dspTime);
    [DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)]
    static extern int ManhattanReceive(IntPtr manhattan, StringBuilder message, int capacity);
    //[DllImport("manhattan", CallingConvention = CallingConvention.Cdecl)] 
    //unsafe delegate int ManhattanRequest(IntPtr manhattan, string request, StringBuilder reply, int reply_size);
#endif

    void Awake() {
#if UNITY_EDITOR //|| !IOS
        // load library dynamically on editor play
        if (_library != IntPtr.Zero) return;
#if UNITY_EDITOR_OSX
        _library = Native.LoadLibrary("Assets/Plugins/macOS/manhattan");
#elif UNITY_EDITOR_WIN
        _library = Native.LoadLibrary("Assets/Plugins/Windows/x64/manhattan");
#endif
        if (_library == IntPtr.Zero) {
            int errno = Marshal.GetLastWin32Error();
            Debug.LogError("Manhattan: Failed to load library (Error: " + errno + ").");
            return;
        }
        if(!Check())
            Debug.Log("Manhattan: Using dynamic library (Check: Fail)");
        
        if (DebugOutput && Native.EnableDebugging(_library))
            Debug.Log("Manhattan: Debugging enabled.");
#endif
        _time = UpdateInterval;

        foreach (GameObject obj in Listeners) {
            Listener l = obj.GetComponent<Listener>();
            if (l != null)
                _listeners.Add(l);
        }
    }

    void Start()
    {
        int status = 0;

#if UNITY_EDITOR //|| !IOS
        if (_library == IntPtr.Zero || active) return;
#endif

        // load engine
        _manhattan = Create(PlaybackMode == Mode.LiveEditing);
        if (_manhattan == IntPtr.Zero) {
            if (PlaybackMode == Mode.LiveEditing)
                Debug.LogError("Manhattan: Failed to initialise live mode.");
            else
                Debug.LogError("Manhattan: Failed to create renderer.");
            return;
        }

        active = true;

        if (Test() != 1)
            Debug.LogError("Manhattan: Pipe send test failed.");


        System.Threading.Thread.Sleep(1000); // wait for server to reply

        string message = Receive();
        if (message != "pong") {
            Debug.LogError("Manhattan: Pipe receive test failed.");
            Debug.Log("Received:" + message);
        }
        
        if (PlaybackMode == Mode.LiveEditing) {
            Debug.Log("Manhattan: Using live mode.");
        } else {
            int[] CHANNELS = { 0, 1, 2, 4, 5, 6, 8, 2 }; // (see AudioSpeakerMode)
            Config(AudioSettings.outputSampleRate, CHANNELS[(int)AudioSettings.speakerMode]);
            Debug.Log("Manhattan: Engine started.");

            status = Open(Application.streamingAssetsPath + "/" + Filename);
            if (status == 2) // 2 = LOAD_SUCCESS
                Debug.Log("Manhattan: Music loaded successfully <color=grey>(" + Application.streamingAssetsPath + "/" + Filename + ")</color>");
            else
                Debug.Log("Manhattan: Failed to load music (Error: " + status + ").");
        }

        if (AutoPlay) {
            status = Play();
            if (status == 1)
                Debug.Log("Manhattan: Playback started.");
            else
                Debug.Log("Manhattan: Playback unable to start.");
        }
    }

    void Update()
    {
//#if UNITY_EDITOR //|| !IOS
        _time -= Time.deltaTime;
        if (_time <= 0) {
            _time = UpdateInterval;
        }

        if(active) {
            if (PlaybackMode == Mode.LiveEditing) {
                // receive messages directly from Manhattan
                string message = Receive();
                if (message != null) {
                    string[] messages = message.Split('\n');
                    foreach (string m in messages)
                        Input(m);
                }
            } else {
                // process messages from audio thread
                foreach (string message in _messages) {
                    string[] messages = message.Split('\n');
                    foreach (string m in messages)
                        Input(m);
                }
                _messages.Clear();
            }
        }
//#endif
    }

    void OnDestroy()
    {
        active = false;
        if (_manhattan == IntPtr.Zero) return;

        IntPtr manhattan_ptr = _manhattan;
        _manhattan = IntPtr.Zero;

        Play();

#if UNITY_EDITOR //|| !IOS
        if (_library == IntPtr.Zero) return;
        Native.Invoke<ManhattanDestroy>(_library, manhattan_ptr);
#else
        ManhattanDestroy(manhattan_ptr);
#endif
        if (PlaybackMode == Mode.LiveEditing)
            Debug.Log("Manhattan: Live mode stopped.");
        else
            Debug.Log("Manhattan: Engine stopped.");
    }

#if UNITY_EDITOR //|| !IOS
    void OnApplicationQuit()
    {
        active = false;

        if (_manhattan != IntPtr.Zero)
            OnDestroy();

        if (_library == IntPtr.Zero) return;
        Native.DisableDebugging(_library);
        if(!Native.FreeLibrary(_library))
            Debug.Log("Manhattan: Could not be unloaded.");

        _library = IntPtr.Zero;
    }
#endif

    // Process Audio Buffer
    unsafe void OnAudioFilterRead(float[] data, int channels)
    {
        if (!active || PlaybackMode == Mode.LiveEditing)
            return;

        // generate audio
        IntPtr buffer;
        fixed (float* ptr = data) buffer = (IntPtr)ptr;
        Process(buffer, data.Length >> 1, channels, AudioSettings.dspTime);

        // apply output gain
        for (int s = 0; s < data.Length; s++)
            data[s] *= OutputLevel;

        // store messages from Manhattan (for later processing in Update())
        string message = Receive();
        if (message != null)
            _messages.Add(message);
    }
}