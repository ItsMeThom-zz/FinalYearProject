using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;




public static class ef {

    public static void start() {
        foldInd = 0;
        searchBarInd = 0;
        //EditorGUILayout.BeginHorizontal();
        lineOpen = false;
    }

    static bool lineOpen = false;
    static int selectedFold = -1;
    public static string searchBarInput = "";
    static int selectedSearchBar = 0;
    static int foldInd;
    static int searchBarInd;
    public static bool searchInFocus = false;
  

    public static List<int> search (List<string> from) {
        checkLine();
        searchInFocus = false;
        List<int> inds = new List<int>();
        string FullName = "ef_SRCH" + searchBarInd;
        string tmp = "";
       GUI.SetNextControlName(FullName);
        if (selectedSearchBar == searchBarInd)
            searchBarInput = EditorGUILayout.TextField(searchBarInput);
        else
            tmp = EditorGUILayout.TextField(tmp);

        if (GUI.GetNameOfFocusedControl() == FullName) {
            selectedSearchBar = searchBarInd;
            searchInFocus = true;
        }

        if (selectedSearchBar == searchBarInd) {
            if (tmp.Length > 0) searchBarInput = tmp;
            selectedSearchBar = searchBarInd;

           

            int lim = searchBarInput.Length < 2 ? Mathf.Min(10, from.Count) : from.Count;

            for (int i=0; i< lim; i++) 
                if (String.Compare(searchBarInput, from[i]) == 0)
                    inds.Add(i);

            ef.newLine();
            if ((lim < from.Count) && (searchInFocus)) ef.write("showing " + lim + " out of " + from.Count);

        }

        searchBarInd++;
        return inds;
    }

    public static bool editPowOf2(ref int i, int min, int max) {
        checkLine();
        int before = i;
        i = Mathf.ClosestPowerOfTwo((int)Mathf.Clamp(EditorGUILayout.IntField(i), min, max));
        return (i != before);
    }

    public static bool isFoldedOut = false;

    public static bool foldout(string txt, ref bool state)
    {
        checkLine();
        state = EditorGUILayout.Foldout(state, txt);
        isFoldedOut = state;
        return isFoldedOut;
    }

    public static bool foldout(string txt, ref int selected, int current) {
        checkLine();

        isFoldedOut = (selected == current);

        if (EditorGUILayout.Foldout(isFoldedOut, txt))
            selected = current;
        else
            if (isFoldedOut) selected = -1;

        isFoldedOut = selected == current;

        return isFoldedOut;
    }

    public static bool foldout (string txt) {
        checkLine();

        isFoldedOut = foldout(txt, ref selectedFold, foldInd);
        
        foldInd++;

        return isFoldedOut;
    }

    public static void foldIn() {
        foldInd = -1;
    }

  
    public static bool select(ref int no, string[] from, int width)   {
        checkLine();

        int newNo = EditorGUILayout.Popup(no, from, GUILayout.MaxWidth(width));
        if (newNo != no)
        {
            no = newNo;
            return true;
        }
        return false;
        //to = from[repName];
    }

    public static bool select(ref int no, string[] from)   {
        checkLine();

        int newNo = EditorGUILayout.Popup(no, from);
        if (newNo != no) {
            no = newNo;
            return true;
        }
        return false;
            //to = from[repName];
    }

    public static bool select(ref int no, Texture[] tex) {
        if (tex.Length == 0) return false;

        checkLine();
        int before = no;
        List<string> tnames = new List<string>();
        List<int> tnumbers = new List<int>();

        int curno = 0;
        for (int i = 0; i < tex.Length; i++)
            if (tex[i] != null) {
                tnumbers.Add(i);
                tnames.Add(i + ": " + tex[i].name);
                if (no == i) curno = tnames.Count - 1;
            }

        curno = EditorGUILayout.Popup(curno, tnames.ToArray());

        no = tnumbers[curno];

        return (before != no);
    }

  

    public static void tab()
    {
        checkLine();
        EditorGUILayout.Space( );
    }

    public static void line() {
        checkLine();
        EditorGUILayout.Separator();
        newLine();
    }

    public static void editOrselect(ref string to, string[] from) {
        checkLine();
        to = EditorGUILayout.TextField(to);
        int repName = EditorGUILayout.Popup(-1, from);
        if (repName != -1)
            to = from[repName];
    }

    public static bool edit(ref float val) {
        checkLine();
        float before = val;
        val = EditorGUILayout.FloatField(val);
        return (val != before);
    }

    public static bool edit(ref int val, int min, int max) {
        checkLine();
        float before = val;
        val = EditorGUILayout.IntSlider(val, min, max); //Slider(val, min, max);
        return (val != before);
    }

    public static bool edit(ref float val, float min, float max )
    {
        checkLine();
        float before = val;
        val = EditorGUILayout.Slider(val, min, max);
        return (val != before);
    }

    public static bool edit (ref Color col) {

        checkLine();
        Color before = col;
        col = EditorGUILayout.ColorField(col);

        return (before.Equals(col) == false);

    }

    public static bool edit(ref int val) {
        checkLine();
        int pre = val;
        val = EditorGUILayout.IntField(val);
        return val != pre;
    }

    public static bool edit(ref Vector2 val) {
        checkLine();
        bool modified = false;
        modified |= ef.edit(ref val.x);
        modified |= ef.edit(ref val.y);
        return modified;
    }

    public static bool toggleInt(ref int val)
    {
        checkLine();
        bool before = val>0;
        if (ef.toggle(ref before)) {
            val = before ? 1 : 0;
            return true;
        }
        return false;
    }

    public static bool toggle (ref bool val) {
        checkLine();
        bool before = val;
        val = EditorGUILayout.Toggle(val);
        return (before != val);
    }

    

    public static bool edit (ref string text) {
        checkLine();
        string before = text;
        text = EditorGUILayout.TextField(text);
        return (String.Compare(before, text) != 0);
    }

    public static bool edit(ref string[] texts, int no)
    {
        checkLine();
        string before = texts[no];
        texts[no] = EditorGUILayout.TextField(texts[no]);
        return (String.Compare(before, texts[no]) != 0);
    }

    public static bool edit(List<string> texts, int no)
    {
        checkLine();
        string before = texts[no];
        texts[no] = EditorGUILayout.TextField(texts[no]);
        return (String.Compare(before, texts[no]) != 0);
    }
    // 
    public static bool Bttn(string txt, int width)
    {
        checkLine();
        return GUILayout.Button(txt, GUILayout.MaxWidth(width));
    }

    public static bool Bttn(string txt) {
        checkLine();
        return GUILayout.Button(txt);
    }

    public static bool Bttn(Texture img) {
        checkLine();
        return GUILayout.Button(img);
    }

    public static void write (string text, int width) {

        checkLine();

        EditorGUILayout.LabelField(text, GUILayout.MaxWidth(width));
    }

    public static void write(string text)
    {
        checkLine();

        EditorGUILayout.LabelField(text);
    }

    public static void checkLine()
    {
        if (!lineOpen) {
            EditorGUILayout.BeginHorizontal();
            lineOpen = true;
        }
    }

    public static void newLine() {
        if (lineOpen) {
            lineOpen = false;
            EditorGUILayout.EndHorizontal();
        }
    }

    public static void ShowTeture(Texture tex)
    {
        checkLine();
        //Texture2D tex = (Texture2D)
        EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);
    }

    public static bool edit<T> (ref T field, string name) where T: UnityEngine.Object{
        checkLine();
        T tmp = field;
        field = (T)EditorGUILayout.ObjectField(name, field,typeof(T), true);
        return tmp != field;
        }


    public static void SetDefine(string val, bool to) {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (defines.Contains(val) == to) return;

        if (to)
            defines += " ; " + val;
        else
            defines = defines.Replace(val, "");  
        

        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
    }

    public static bool GetDefine(string define) {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        return defines.Contains(define);
    }



}
