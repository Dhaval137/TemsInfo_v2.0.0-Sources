using System.Collections.Generic;
using System.IO;

namespace Ectn.Utilities.TemsInfo {

    public partial class TemsInfoBinary3D {

        #region Props

        public int Count {
            get {
                return profiles.Count;
            }
        }

        public List<Point> this[int idx] {
            get {
                return profiles[idx];
            }
        }

        #endregion Props

        #region Fields

        private List<List<Point>> profiles;

        #endregion Fields

        public TemsInfoBinary3D(byte[] data) {
            using (MemoryStream memory = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(memory)) {
                    int countProfiles = reader.ReadInt32();
                    profiles = new List<List<Point>>(countProfiles);
                    for (int idxProfile = 0; idxProfile < countProfiles; idxProfile++) {
                        int countPoints = reader.ReadInt32();
                        profiles.Add(new List<Point>(countPoints));
                        for (int idxPoint = 0; idxPoint < countPoints; idxPoint++) {
                            profiles[idxProfile].Add(new Point(reader));
                        }
                    }
                }
            }
        }

        #region Public Access

        public int CountPoints() {
            int result = 0;
            for (int idxProfile = 0; idxProfile < profiles.Count; idxProfile++) {
                result += profiles[idxProfile].Count;
            }
            return result;
        }

        #endregion Public Access

        #region Utils

        public partial class Point {

            #region Props

            public double X {
                get;
                private set;
            }

            public double Y {
                get;
                private set;
            }

            public double Z {
                get;
                private set;
            }

            #endregion Props

            public Point(BinaryReader reader) {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
            }
        }

        #endregion Utils
    }
}