using Bunkum.Core.Configuration;

namespace Refresh.Core.Configuration;

public class DryArchiveConfig : Config
{
    public override int CurrentConfigVersion => 1;
    public override int Version { get; set; }
    
    protected override void Migrate(int oldVer, dynamic oldConfig)
    {
        
    }
    
    public bool Enabled { get; set; }
    public string Location { get; set; } = "/var/dry/";
    public bool UseFolderNames { get; set; } = true;
    public bool RemoteEnabled { get; set; } = true;
    public string RemoteBaseUrl { get; set; } = "https://archive.org/download/";
    
#if DEBUG
    // ReSharper disable once InconsistentNaming
    public bool TemporaryWillBeRemoved_UseProductionRefreshData { get; set; } = false;
#endif
}
