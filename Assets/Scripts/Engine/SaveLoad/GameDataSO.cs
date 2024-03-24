using UnityEngine;
using System.Collections.Generic;
public interface IGameData
{
    string DataName { get; set; }
    int DataVersion { get; set; }
    string UserId { get; set; }
    string SaveTimestamp { get; set ; }
}

[CreateAssetMenu(fileName = "GameData", menuName = "Save Load/GameData", order = 1)]
public class GameDataSO : ScriptableObject, IGameData
{
    public virtual string DataName { get ; set ; }
    public virtual int DataVersion { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public virtual string UserId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public virtual string SaveTimestamp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }



}
