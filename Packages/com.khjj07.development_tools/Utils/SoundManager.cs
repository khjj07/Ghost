using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
//�� ���ӿ����� �Ÿ��� ����� ������ ũ�⸦ ������ �ʿ䰡 ���⿡ �ϳ��� AudioSource�� AudioClip���� �������� �����ų ���̴�.
//��������� ������ AudioSource�� ȿ������ ������ AudioSource�� SoundManager�� �ڽ� ������Ʈ�� ����


[Serializable]
public class PlayerStruct
{
    public AudioSource Instance;
    public float Volume =1f;
    public bool Loop = false;
    public List<SoundStruct> ClipList = new List<SoundStruct>();
    /*
    public PlayerStruct(AudioSource instance)
    {
        Instance = instance;
        Volume = 1f;
    }*/
    public PlayerStruct(AudioSource instance, float volume, bool loop)
    {
        Instance = instance;
        Volume = volume;
        Loop = loop;
    }
    public void PlaySound(string name, float volume = 1f)
    {
        if (Loop)
        {
            Instance.volume = Volume * volume;
            Instance.clip = ClipList.Find(x => x.Name.Equals(name)).Clip;
            Instance.Play();
        }
        else
            Instance.PlayOneShot(ClipList.Find(x => x.Name.Equals(name)).Clip, Volume * volume);
    }
}

[Serializable]
public class SoundStruct
{
    public string Name;
    public AudioClip Clip;
    public SoundStruct(string name, AudioClip clip)
    {
        Name = name;
        Clip = clip;
    }
}

public class SoundManager : Singleton<SoundManager>
{
    private bool DontDestroy = true;
    [SerializeField]
    private List<PlayerStruct> Player;



    private void Awake()
    {
        if (DontDestroy)
            DontDestroyOnLoad(this.gameObject);
    }

    public void PlaySound(int index, string name, float volume = 1f)
    {
        PlayerStruct player = Player[index];
        player.PlaySound(name,volume);
    }
}