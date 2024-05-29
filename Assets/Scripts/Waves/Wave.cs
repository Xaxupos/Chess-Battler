using System.Collections.Generic;
using VInspector;

[System.Serializable]
public class Wave
{
    public List<ChessFigureType> chessFigures = new List<ChessFigureType>();   

    public void InitWave(SerializedDictionary<ChessFigureType, int> seededData)
    {
        chessFigures.Clear();

        foreach (var kvp in seededData)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                chessFigures.Add(kvp.Key);
            }
        }
    }
}