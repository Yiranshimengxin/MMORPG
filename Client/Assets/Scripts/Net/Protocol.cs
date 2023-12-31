// <auto-generated>
//   This file was generated by a tool; you should avoid making direct changes.
//   Consider using 'partial classes' to extend these types
//   Input: protocol.proto
// </auto-generated>

#region Designer generated code
#pragma warning disable CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace Pb
{

    [global::ProtoBuf.ProtoContract()]
    public partial class PBNetMsg : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"proto")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Proto { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2, Name = @"payload")]
        public byte[] Payload { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"session")]
        public int Session { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBLoginReq : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"account")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Account { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2, Name = @"password")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Password { get; set; } = "";

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBLoginRsp : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"uid")]
        public long Uid { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"name")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; } = "";

        [global::ProtoBuf.ProtoMember(3)]
        public int sceneId { get; set; }

        [global::ProtoBuf.ProtoMember(4, Name = @"items")]
        public global::System.Collections.Generic.List<PBItem> Items { get; } = new global::System.Collections.Generic.List<PBItem>();

        [global::ProtoBuf.ProtoMember(5, Name = @"money")]
        public long Money { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBEnterSceneReq : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SceneObject : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int objId { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"type")]
        public int Type { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"hp")]
        public int Hp { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int maxHp { get; set; }

        [global::ProtoBuf.ProtoMember(5, Name = @"mp")]
        public int Mp { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public int maxMp { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public float positionX { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public float positionY { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public float positionZ { get; set; }

        [global::ProtoBuf.ProtoMember(10, Name = @"level")]
        public int Level { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBEnterSceneRsp : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<SceneObject> sceneObjects { get; } = new global::System.Collections.Generic.List<SceneObject>();

        [global::ProtoBuf.ProtoMember(2, Name = @"self")]
        public SceneObject Self { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBEnterSceneNotify : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"object")]
        public SceneObject Object { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBAttackReq : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int objId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBAttackRsp : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int AttackObjId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int AttackedObjId { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"damage")]
        public int Damage { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBAttackNotify : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int AttackObjId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int AttackedObjId { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"damage")]
        public int Damage { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBObjectDieNotify : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int objId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBUpdateResourceNotify : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"exp")]
        public long Exp { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"level")]
        public int Level { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"money")]
        public long Money { get; set; }

        [global::ProtoBuf.ProtoMember(4, Name = @"items")]
        public global::System.Collections.Generic.List<PBItem> Items { get; } = new global::System.Collections.Generic.List<PBItem>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBMoveReq : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public float positionX { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public float positionY { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public float positionZ { get; set; }

        [global::ProtoBuf.ProtoMember(4, Name = @"speed")]
        public float Speed { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBMoveNotify : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int objId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public float positionX { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public float positionY { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public float positionZ { get; set; }

        [global::ProtoBuf.ProtoMember(5, Name = @"speed")]
        public float Speed { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBSyncPositionReq : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public float positionX { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public float positionY { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public float positionZ { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBItem : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int itemId { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"count")]
        public int Count { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBUseItemReq : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int itemId { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"count")]
        public int Count { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PBUseItemRsp : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"items")]
        public global::System.Collections.Generic.List<PBItem> Items { get; } = new global::System.Collections.Generic.List<PBItem>();

    }

}

#pragma warning restore CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
#endregion
