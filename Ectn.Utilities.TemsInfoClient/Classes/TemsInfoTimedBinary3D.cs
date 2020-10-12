using System;
using System.Collections.Generic;
using System.IO;

namespace Ectn.Utilities.TemsInfo {

    public partial class TemsInfoTimedBinary3D {

        #region Props

        public int Count {
            get {
                return profiles.Count;
            }
        }

        public Profile this[int idx] {
            get {
                return profiles[idx];
            }
        }

        #endregion Props

        #region Fields

        private List<Profile> profiles;

        #endregion Fields

        public TemsInfoTimedBinary3D(byte[] data) {
            using (MemoryStream memory = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(memory)) {
                    int countProfiles = reader.ReadInt32();
                    profiles = new List<Profile>(countProfiles);
                    for (int idxProfile = 0; idxProfile < countProfiles; idxProfile++) {
                        profiles.Add(new Profile(reader));
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

        public partial class Profile {

            #region Props

            public DateTime Timestamp {
                get;
                private set;
            }

            public int Count {
                get {
                    return points.Count;
                }
            }

            public TemsInfoBinary3D.Point this[int idx] {
                get {
                    return points[idx];
                }
            }

            #endregion Props

            #region Fields

            private List<TemsInfoBinary3D.Point> points;

            #endregion Fields

            public Profile(BinaryReader reader) {
                Timestamp = new DateTime(reader.ReadInt64());
                int countPoints = reader.ReadInt32();
                points = new List<TemsInfoBinary3D.Point>(countPoints);
                for (int idxPoint = 0; idxPoint < countPoints; idxPoint++) {
                    points.Add(new TemsInfoBinary3D.Point(reader));
                }
            }
        }

        #endregion Utils
    }
}