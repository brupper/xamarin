using System;
using System.Collections.Generic;

namespace Brupper.Maui.Services;

[Obsolete("User MAUI network")]
public class ConnectivityService : IConnectivity
{
    public NetworkAccess NetworkAccess => Connectivity.NetworkAccess;

    public IEnumerable<ConnectionProfile> ConnectionProfiles => Connectivity.ConnectionProfiles;

    public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged
    {
        add { Connectivity.ConnectivityChanged += value; }
        remove { Connectivity.ConnectivityChanged -= value; }
    }
}
