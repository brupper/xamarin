using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Services
{
    public class ConnectivityService : IConnectivity
    {
        public NetworkAccess NetworkAccess
            => Connectivity.NetworkAccess;

        public IEnumerable<ConnectionProfile> ConnectionProfiles => Connectivity.ConnectionProfiles;

        public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged
        {
            add { Connectivity.ConnectivityChanged += value; }
            remove { Connectivity.ConnectivityChanged -= value; }
        }
    }
}
