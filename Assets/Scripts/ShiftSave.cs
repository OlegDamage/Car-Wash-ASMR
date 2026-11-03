using System;
using UnityEngine;

public static class ShiftSave
{
    private const string Key = "CarWashASMR.CompletedShiftStats.v1";

    [Serializable]
    public class Statistics
    {
        public int schemaVersion;
        public int completedShifts;
        public long totalCars;
        public int lastShiftCars;
        public string lastResultId;
    }

    public static bool TryLoad(out Statistics data, out string error)
    {
        data = null;
        error = null;
        try
        {
            if (!PlayerPrefs.HasKey(Key))
            {
                data = new Statistics { schemaVersion = 1, lastResultId = "" };
                return true;
            }
            string json = PlayerPrefs.GetString(Key);
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("Saved shift data is empty. It was not reset.");
            Statistics loaded = JsonUtility.FromJson<Statistics>(json);
            if (!IsValid(loaded))
                throw new InvalidOperationException("Saved shift data is invalid or has an unsupported version. It was not reset.");
            data = loaded;
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
    }

    public static bool TryCommit(string resultId, int cars, out Statistics saved, out string error)
    {
        saved = null;
        error = null;
        if (cars < 0 || !Guid.TryParseExact(resultId, "N", out Guid parsedId))
        {
            error = "Invalid completed-shift result.";
            return false;
        }
        if (!TryLoad(out Statistics previous, out error)) return false;
        try
        {
            if (previous.lastResultId == resultId)
            {
                if (previous.lastShiftCars != cars)
                    throw new InvalidOperationException("The same shift result has conflicting car counts.");
                PlayerPrefs.Save();
                saved = previous;
                return true;
            }
            Statistics next = new Statistics
            {
                schemaVersion = 1,
                completedShifts = checked(previous.completedShifts + 1),
                totalCars = checked(previous.totalCars + cars),
                lastShiftCars = cars,
                lastResultId = resultId
            };
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(next));
            PlayerPrefs.Save();
            saved = next;
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
    }

    private static bool IsValid(Statistics data)
    {
        if (data == null || data.schemaVersion != 1 || data.completedShifts < 0 ||
            data.totalCars < 0 || data.lastShiftCars < 0 || data.totalCars < data.lastShiftCars)
            return false;
        if (data.completedShifts == 0)
            return data.totalCars == 0 && data.lastShiftCars == 0 &&
                string.IsNullOrEmpty(data.lastResultId);
        return Guid.TryParseExact(data.lastResultId, "N", out Guid parsedId);
    }
}
