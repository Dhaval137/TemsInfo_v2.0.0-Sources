namespace Ectn.Utilities.TemsInfo {

    public enum TemsInfoMessageType : ushort {

        Initialization = 0,

        Information = 1,

        Error = 2,

        Configuration = 3,

        Heartbeat = 4,

        SetSite = 5,

        Vehicle = 102,

        ObjectStart = 200,

        ObjectStop = 201,

        ObjectLocation = 202,

        Status = 300,

        Value = 400,

        Data = 401
    }
}