﻿using System.IO;

namespace DXVcs2Git.Core.GitLab {
    public enum MergeRequestActionType {
        sync,
        testbuild,
    }

    public abstract class MergeRequestActionBase {
        protected internal abstract MergeRequestActionType ActionType { get; }
    }

    public class MergeRequestSyncAction : MergeRequestActionBase {
        public string WatckTask { get; }
        public string SyncTask { get; }
        public MergeRequestSyncAction(string watchTask, string syncTask) {
            WatckTask = watchTask;
            SyncTask = syncTask;
        }
        protected internal override MergeRequestActionType ActionType => MergeRequestActionType.sync;
    }

    public class MergeRequestTestBuildTask : MergeRequestActionBase {
        public string Sha { get; }

        public MergeRequestTestBuildTask(string sha) {
            Sha = sha;
        }

        protected internal override MergeRequestActionType ActionType => MergeRequestActionType.testbuild;
    }

    public class MergeRequestOptions {
        public static string ConvertToString(MergeRequestOptions options) {
            if (options == null)
                return null;

            using (MemoryStream stream = new MemoryStream()) {
                Serializer.Serialize(stream, options);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
        public static MergeRequestOptions ConvertFromString(string str) {
            if (string.IsNullOrEmpty(str))
                return null;
            using (MemoryStream stream = new MemoryStream()) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(str);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    return Serializer.Deserialize<MergeRequestOptions>(stream);
                }
            }
        }
        MergeRequestActionBase action;
        public MergeRequestActionType ActionType { get; set; }
        public MergeRequestActionBase Action {
            get { return action; }
            set {
                if (action == value)
                    return;
                action = value;
                ActionType = value?.ActionType ?? MergeRequestActionType.sync;
            }
        }

        public MergeRequestOptions(MergeRequestActionBase action = null) {
            Action = action;
        }
    }
}