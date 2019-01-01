namespace XPCF
{
    public partial class UUID
    {
        public static implicit operator UUID(string uuid) { return xpcf.toUUID(uuid); }

        public static UUID Create(string uuid) { return xpcf.toUUID(uuid); }
    }
}
