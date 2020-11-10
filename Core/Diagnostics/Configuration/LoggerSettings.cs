using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public class LoggerSettings
    {
        #region Members

        protected UserGroupLoggerSection appropriateUserGroup;
        protected bool isCustomerAccountInUserGroup;

        #endregion

        #region Properties

        public List<UserGroupLoggerSection> UserGroupLoggerSections { get; } = new List<UserGroupLoggerSection>();

        [XmlIgnore]
        public Dictionary<LogProviderName, LogLevel> CustomerAccountProviders { get; } = new Dictionary<LogProviderName, LogLevel>
        {
            {LogProviderName.Local, LogLevel.Medium },
            {LogProviderName.Remote, LogLevel.Medium },
        };


        #endregion

        public LogTag GetLocalLogLevel()
        {
            if (isCustomerAccountInUserGroup && appropriateUserGroup != null && appropriateUserGroup.UserGroupsProviders.ContainsKey(LogProviderName.Local))
            {
                return appropriateUserGroup.UserGroupsProviders[LogProviderName.Local].LogLevelToLogTag();
            }

            return CustomerAccountProviders[LogProviderName.Local].LogLevelToLogTag();
        }

        public LogTag GetRemoteLogLevel()
        {
            if (isCustomerAccountInUserGroup && appropriateUserGroup?.UserGroupsProviders != null && appropriateUserGroup.UserGroupsProviders.ContainsKey(LogProviderName.Remote))
            {
                return appropriateUserGroup.UserGroupsProviders[LogProviderName.Remote].LogLevelToLogTag();
            }

            return CustomerAccountProviders[LogProviderName.Remote].LogLevelToLogTag();
        }

        public void SetLoggerLevels(IUser user)
        {
            isCustomerAccountInUserGroup = (UserGroupLoggerSections == null || !UserGroupLoggerSections.Any()
                   || IsCustomerAccountInUserGroup(UserGroupLoggerSections, user));

            SetLogTagByAppropriateLogLevel(GetLocalLogLevel(), GetRemoteLogLevel());
            SetRemoteLogLevelName();

            Logger.Current.UpdateProvidersByConfig();
        }

        public bool IsCustomerAccountInUserGroup(List<UserGroupLoggerSection> userGroupLoggerSections, IUser user)
        {
            isCustomerAccountInUserGroup = false;

            foreach (var userGroupLoggerSection in userGroupLoggerSections)
            {
                foreach (var userId in userGroupLoggerSection.UserIds)
                {
                    bool userIdOrNameMatched = userId == user.Id || userId == user.UserName;
                    if (userIdOrNameMatched)
                    {
                        isCustomerAccountInUserGroup = true;
                        appropriateUserGroup = userGroupLoggerSection;

                        return isCustomerAccountInUserGroup;
                    }
                }
            }

            return isCustomerAccountInUserGroup;
        }

        private void SetRemoteLogLevelName()
        {
            if (isCustomerAccountInUserGroup && appropriateUserGroup != null && appropriateUserGroup.UserGroupsProviders.ContainsKey(LogProviderName.Remote) && appropriateUserGroup.UserGroupsProviders.ContainsKey(LogProviderName.Local))
            {
                Logger.Current.LocalLogLevelName = appropriateUserGroup.UserGroupsProviders[LogProviderName.Local].ToString();
                Logger.Current.RemoteLogLevelName = appropriateUserGroup.UserGroupsProviders[LogProviderName.Remote].ToString();
            }
            else
            {
                Logger.Current.LocalLogLevelName = CustomerAccountProviders[LogProviderName.Local].ToString();
                Logger.Current.RemoteLogLevelName = CustomerAccountProviders[LogProviderName.Remote].ToString();
            }
        }

        private void SetLogTagByAppropriateLogLevel(LogTag localTag, LogTag remoteLogTag)
        {
            Logger.Current.LocalLogTag = localTag;
            Logger.Current.RemoteLogTag = remoteLogTag;
        }
    }
}
