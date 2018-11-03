namespace PHP.Library.TypeSystem
{
    public interface IScript
    {
        NormalizedPath GetScriptPath ();
        NormalizedPath GetScriptBaseDirectory ();
    }
}
