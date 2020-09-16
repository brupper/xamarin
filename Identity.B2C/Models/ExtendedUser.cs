﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Brupper.Identity.B2C.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtendedUser : GraphUser
    {
        /// <summary> The User constructor </summary>
        public ExtendedUser()
        {
            ODataType = "microsoft.graph.user";
        }

        /// <summary> Gets or sets account enabled. true if the account is enabled; otherwise, false. This property is required when a user is created. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "accountEnabled", Required = Newtonsoft.Json.Required.Default)]
        public bool? AccountEnabled { get; set; }
        /// <summary> Gets or sets age group. Sets the age group of the user. Allowed values: null, minor, notAdult and adult. Refer to the legal age group property definitions for further information.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ageGroup", Required = Newtonsoft.Json.Required.Default)]
        public string AgeGroup { get; set; }
        /// <summary> Gets or sets business phones. The telephone numbers for the user. NOTE: Although this is a string collection, only one number can be set for this property.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "businessPhones", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> BusinessPhones { get; set; }
        /// <summary> Gets or sets company name. The company name which the user is associated. This property can be useful for describing the company that an external user comes from.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "companyName", Required = Newtonsoft.Json.Required.Default)]
        public string CompanyName { get; set; }
        /// <summary> Gets or sets consent provided for minor. Sets whether consent has been obtained for minors. Allowed values: null, granted, denied and notRequired. Refer to the legal age group property definitions for further information.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "consentProvidedForMinor", Required = Newtonsoft.Json.Required.Default)]
        public string ConsentProvidedForMinor { get; set; }
        /// <summary> Gets or sets creation type. Indicates whether the user account was created as a regular school or work account (null), an external account (Invitation), a local account for an Azure Active Directory B2C tenant (LocalAccount) or self-service sign-up using email verification (EmailVerified). Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "creationType", Required = Newtonsoft.Json.Required.Default)]
        public string CreationType { get; set; }
        /// <summary> Gets or sets department. The name for the department in which the user works. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "department", Required = Newtonsoft.Json.Required.Default)]
        public string Department { get; set; }
        /// <summary> Gets or sets display name. The name displayed in the address book for the user. This is usually the combination of the user's first name, middle initial and last name. This property is required when a user is created and it cannot be cleared during updates. Supports $filter and $orderby.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "displayName", Required = Newtonsoft.Json.Required.Default)]
        public string DisplayName { get; set; }
        /// <summary> Gets or sets employee id. The employee identifier assigned to the user by the organization. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "employeeId", Required = Newtonsoft.Json.Required.Default)]
        public string EmployeeId { get; set; }
        /// <summary> Gets or sets fax number. The fax number of the user.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "faxNumber", Required = Newtonsoft.Json.Required.Default)]
        public string FaxNumber { get; set; }
        /// <summary> Gets or sets im addresses. The instant message voice over IP (VOIP) session initiation protocol (SIP) addresses for the user. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "imAddresses", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> ImAddresses { get; set; }
        /// <summary> Gets or sets is resource account. true if the user is a resource account; otherwise, false. Null value should be considered false.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isResourceAccount", Required = Newtonsoft.Json.Required.Default)]
        public bool? IsResourceAccount { get; set; }
        /// <summary> Gets or sets job title. The user’s job title. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jobTitle", Required = Newtonsoft.Json.Required.Default)]
        public string JobTitle { get; set; }
        /// <summary> Gets or sets last password change date time. The time when this Azure AD user last changed their password. The date and time information uses ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "lastPasswordChangeDateTime", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? LastPasswordChangeDateTime { get; set; }
        /// <summary> Gets or sets legal age group classification. Used by enterprise applications to determine the legal age group of the user. This property is read-only and calculated based on ageGroup and consentProvidedForMinor properties. Allowed values: null, minorWithOutParentalConsent, minorWithParentalConsent, minorNoParentalConsentRequired, notAdult and adult. Refer to the legal age group property definitions for further information.)</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "legalAgeGroupClassification", Required = Newtonsoft.Json.Required.Default)]
        public string LegalAgeGroupClassification { get; set; }
        /// <summary> Gets or sets mail. The SMTP address for the user, for example, 'jeff@contoso.onmicrosoft.com'. Read-Only. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mail", Required = Newtonsoft.Json.Required.Default)]
        public string Mail { get; set; }
        /// <summary> Gets or sets mail nickname. The mail alias for the user. This property must be specified when a user is created. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mailNickname", Required = Newtonsoft.Json.Required.Default)]
        public string MailNickname { get; set; }
        /// <summary> Gets or sets mobile phone. The primary cellular telephone number for the user.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mobilePhone", Required = Newtonsoft.Json.Required.Default)]
        public string MobilePhone { get; set; }
        /// <summary> Gets or sets on premises distinguished name. Contains the on-premises Active Directory distinguished name or DN. The property is only populated for customers who are synchronizing their on-premises directory to Azure Active Directory via Azure AD Connect. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesDistinguishedName", Required = Newtonsoft.Json.Required.Default)]
        public string OnPremisesDistinguishedName { get; set; }
        /// <summary> Gets or sets on premises immutable id. This property is used to associate an on-premises Active Directory user account to their Azure AD user object. This property must be specified when creating a new user account in the Graph if you are using a federated domain for the user’s userPrincipalName (UPN) property. Important: The $ and _ characters cannot be used when specifying this property. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesImmutableId", Required = Newtonsoft.Json.Required.Default)]
        public string OnPremisesImmutableId { get; set; }
        /// <summary> Gets or sets on premises last sync date time. Indicates the last time at which the object was synced with the on-premises directory; for example: '2013-02-16T03:04:54Z'. The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesLastSyncDateTime", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? OnPremisesLastSyncDateTime { get; set; }
        /// <summary> Gets or sets on premises security identifier. Contains the on-premises security identifier (SID) for the user that was synchronized from on-premises to the cloud. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesSecurityIdentifier", Required = Newtonsoft.Json.Required.Default)]
        public string OnPremisesSecurityIdentifier { get; set; }
        /// <summary> Gets or sets on premises sync enabled. true if this object is synced from an on-premises directory; false if this object was originally synced from an on-premises directory but is no longer synced; null if this object has never been synced from an on-premises directory (default). Read-only</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesSyncEnabled", Required = Newtonsoft.Json.Required.Default)]
        public bool? OnPremisesSyncEnabled { get; set; }
        /// <summary> Gets or sets on premises domain name. Contains the on-premises domainFQDN, also called dnsDomainName synchronized from the on-premises directory. The property is only populated for customers who are synchronizing their on-premises directory to Azure Active Directory via Azure AD Connect. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesDomainName", Required = Newtonsoft.Json.Required.Default)]
        public string OnPremisesDomainName { get; set; }
        /// <summary> Gets or sets on premises sam account name. Contains the on-premises samAccountName synchronized from the on-premises directory. The property is only populated for customers who are synchronizing their on-premises directory to Azure Active Directory via Azure AD Connect. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesSamAccountName", Required = Newtonsoft.Json.Required.Default)]
        public string OnPremisesSamAccountName { get; set; }
        /// <summary> Gets or sets on premises user principal name. Contains the on-premises userPrincipalName synchronized from the on-premises directory. The property is only populated for customers who are synchronizing their on-premises directory to Azure Active Directory via Azure AD Connect. Read-only.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesUserPrincipalName", Required = Newtonsoft.Json.Required.Default)]
        public string OnPremisesUserPrincipalName { get; set; }
        /// <summary> Gets or sets password policies. Specifies password policies for the user. This value is an enumeration with one possible value being 'DisableStrongPassword', which allows weaker passwords than the default policy to be specified. 'DisablePasswordExpiration' can also be specified. The two may be specified together; for example: 'DisablePasswordExpiration, DisableStrongPassword'.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "passwordPolicies", Required = Newtonsoft.Json.Required.Default)]
        public string PasswordPolicies { get; set; }
        /// <summary> Gets or sets office location. The office location in the user's place of business.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "officeLocation", Required = Newtonsoft.Json.Required.Default)]
        public string OfficeLocation { get; set; }
        /// <summary> Gets or sets preferred language. The preferred language for the user. Should follow ISO 639-1 Code; for example 'en-US'.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "preferredLanguage", Required = Newtonsoft.Json.Required.Default)]
        public string PreferredLanguage { get; set; }
        /// <summary> Gets or sets proxy addresses. For example: ['SMTP: bob@contoso.com', 'smtp: bob@sales.contoso.com'] The any operator is required for filter expressions on multi-valued properties. Read-only, Not nullable. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "proxyAddresses", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> ProxyAddresses { get; set; }
        /// <summary> Gets or sets show in address list. true if the Outlook global address list should contain this user, otherwise false. If not set, this will be treated as true. For users invited through the invitation manager, this property will be set to false.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "showInAddressList", Required = Newtonsoft.Json.Required.Default)]
        public bool? ShowInAddressList { get; set; }
        /// <summary> Gets or sets sign in sessions valid from date time. Any refresh tokens or sessions tokens (session cookies) issued before this time are invalid, and applications will get an error when using an invalid refresh or sessions token to acquire a delegated access token (to access APIs such as Microsoft Graph).  If this happens, the application will need to acquire a new refresh token by making a request to the authorize endpoint. Read-only. Use revokeSignInSessions to reset.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "signInSessionsValidFromDateTime", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? SignInSessionsValidFromDateTime { get; set; }
        /// <summary> Gets or sets usage location. A two letter country code (ISO standard 3166). Required for users that will be assigned licenses due to legal requirement to check for availability of services in countries.  Examples include: 'US', 'JP', and 'GB'. Not nullable. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "usageLocation", Required = Newtonsoft.Json.Required.Default)]
        public string UsageLocation { get; set; }
        /// <summary> Gets or sets user principal name. The user principal name (UPN) of the user. The UPN is an Internet-style login name for the user based on the Internet standard RFC 822. By convention, this should map to the user's email name. The general format is alias@domain, where domain must be present in the tenant’s collection of verified domains. This property is required when a user is created. The verified domains for the tenant can be accessed from the verifiedDomains property of organization. Supports $filter and $orderby.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userPrincipalName", Required = Newtonsoft.Json.Required.Default)]
        public string UserPrincipalName { get; set; }
        /// <summary> Gets or sets user type. A string value that can be used to classify user types in your directory, such as 'Member' and 'Guest'. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "userType", Required = Newtonsoft.Json.Required.Default)]
        public string UserType { get; set; }
        /// <summary> Gets or sets device enrollment limit. The limit on the maximum number of devices that the user is permitted to enroll. Allowed values are 5 or 1000.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deviceEnrollmentLimit", Required = Newtonsoft.Json.Required.Default)]
        public Int32? DeviceEnrollmentLimit { get; set; }
        /// <summary> Gets or sets about me. A freeform text entry field for the user to describe themselves.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "aboutMe", Required = Newtonsoft.Json.Required.Default)]
        public string AboutMe { get; set; }
        /// <summary> Gets or sets birthday. The birthday of the user. The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "birthday", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? Birthday { get; set; }
        /// <summary> Gets or sets hire date. The hire date of the user. The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hireDate", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? HireDate { get; set; }
        /// <summary> Gets or sets interests. A list for the user to describe their interests.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "interests", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> Interests { get; set; }
        /// <summary> Gets or sets my site. The URL for the user's personal site.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mySite", Required = Newtonsoft.Json.Required.Default)]
        public string MySite { get; set; }
        /// <summary> Gets or sets past projects. A list for the user to enumerate their past projects.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pastProjects", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> PastProjects { get; set; }
        /// <summary> Gets or sets preferred name. The preferred name for the user.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "preferredName", Required = Newtonsoft.Json.Required.Default)]
        public string PreferredName { get; set; }
        /// <summary> Gets or sets responsibilities. A list for the user to enumerate their responsibilities.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "responsibilities", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> Responsibilities { get; set; }
        /// <summary> Gets or sets schools. A list for the user to enumerate the schools they have attended.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "schools", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> Schools { get; set; }
        /// <summary> Gets or sets skills. A list for the user to enumerate their skills.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skills", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<string> Skills { get; set; }
    }

    //public class ComplexUser : ExtendedUser
    //{
    //    /// <summary> Gets or sets assigned licenses. The licenses that are assigned to the user. Not nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assignedLicenses", Required = Newtonsoft.Json.Required.Default)]
    //    public IEnumerable<AssignedLicense> AssignedLicenses { get; set; }
    //    /// <summary> Gets or sets assigned plans. The plans that are assigned to the user. Read-only. Not nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assignedPlans", Required = Newtonsoft.Json.Required.Default)]
    //    public IEnumerable<AssignedPlan> AssignedPlans { get; set; }
    //    /// <summary> Gets or sets identities. Represents the identities that can be used to sign in to this user account. An identity can be provided by Microsoft (also known as a local account), by organizations, or by social identity providers such as Facebook, Google, and Microsoft, and tied to a user account. May contain multiple items with the same signInType value. Supports $filter.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "identities", Required = Newtonsoft.Json.Required.Default)]
    //    public IEnumerable<ObjectIdentity> Identities { get; set; }
    //    /// <summary> Gets or sets license assignment states. State of license assignments for this user. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "licenseAssignmentStates", Required = Newtonsoft.Json.Required.Default)]
    //    public IEnumerable<LicenseAssignmentState> LicenseAssignmentStates { get; set; }
    //    /// <summary> Gets or sets on premises extension attributes. Contains extensionAttributes 1-15 for the user. Note that the individual extension attributes are neither selectable nor filterable. For an onPremisesSyncEnabled user, this set of properties is mastered on-premises and is read-only. For a cloud-only user (where onPremisesSyncEnabled is false), these properties may be set during creation or update.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesExtensionAttributes", Required = Newtonsoft.Json.Required.Default)]
    //    public OnPremisesExtensionAttributes OnPremisesExtensionAttributes { get; set; }
    //    /// <summary> Gets or sets on premises provisioning errors. Errors when using Microsoft synchronization product during provisioning.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onPremisesProvisioningErrors", Required = Newtonsoft.Json.Required.Default)]
    //    public IEnumerable<OnPremisesProvisioningError> OnPremisesProvisioningErrors { get; set; }
    //    /// <summary> Gets or sets password profile. Specifies the password profile for the user. The profile contains the user’s password. This property is required when a user is created. The password in the profile must satisfy minimum requirements as specified by the passwordPolicies property. By default, a strong password is required.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "passwordProfile", Required = Newtonsoft.Json.Required.Default)]
    //    public PasswordProfile PasswordProfile { get; set; }
    //    /// <summary> Gets or sets provisioned plans. The plans that are provisioned for the user. Read-only. Not nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "provisionedPlans", Required = Newtonsoft.Json.Required.Default)]
    //    public IEnumerable<ProvisionedPlan> ProvisionedPlans { get; set; }
    //    /// <summary> Gets or sets mailbox settings. Settings for the primary mailbox of the signed-in user. You can get or update settings for sending automatic replies to incoming messages, locale and time zone.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mailboxSettings", Required = Newtonsoft.Json.Required.Default)]
    //    public MailboxSettings MailboxSettings { get; set; }
    //    /// <summary> Gets or sets app role assignments.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "appRoleAssignments", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserAppRoleAssignmentsCollectionPage AppRoleAssignments { get; set; }
    //    /// <summary> Gets or sets owned devices. Devices that are owned by the user. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ownedDevices", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserOwnedDevicesCollectionWithReferencesPage OwnedDevices { get; set; }
    //    /// <summary> Gets or sets registered devices. Devices that are registered for the user. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "registeredDevices", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserRegisteredDevicesCollectionWithReferencesPage RegisteredDevices { get; set; }
    //    /// <summary> Gets or sets manager. The user or contact that is this user’s manager. Read-only. (HTTP Methods: GET, PUT, DELETE.)</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "manager", Required = Newtonsoft.Json.Required.Default)]
    //    public DirectoryObject Manager { get; set; }
    //    /// <summary> Gets or sets direct reports. The users and contacts that report to the user. (The users and contacts that have their manager property set to this user.) Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "directReports", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserDirectReportsCollectionWithReferencesPage DirectReports { get; set; }
    //    /// <summary> Gets or sets member of. The groups and directory roles that the user is a member of. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "memberOf", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserMemberOfCollectionWithReferencesPage MemberOf { get; set; }
    //    /// <summary> Gets or sets created objects. Directory objects that were created by the user. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "createdObjects", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserCreatedObjectsCollectionWithReferencesPage CreatedObjects { get; set; }
    //    /// <summary> Gets or sets oauth2permission grants.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "oauth2PermissionGrants", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserOauth2PermissionGrantsCollectionWithReferencesPage Oauth2PermissionGrants { get; set; }
    //    /// <summary> Gets or sets owned objects. Directory objects that are owned by the user. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ownedObjects", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserOwnedObjectsCollectionWithReferencesPage OwnedObjects { get; set; }
    //    /// <summary> Gets or sets license details. A collection of this user's license details. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "licenseDetails", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserLicenseDetailsCollectionPage LicenseDetails { get; set; }
    //    /// <summary> Gets or sets transitive member of.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "transitiveMemberOf", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserTransitiveMemberOfCollectionWithReferencesPage TransitiveMemberOf { get; set; }
    //    /// <summary> Gets or sets outlook. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "outlook", Required = Newtonsoft.Json.Required.Default)]
    //    public OutlookUser Outlook { get; set; }
    //    /// <summary> Gets or sets messages. The messages in a mailbox or folder. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserMessagesCollectionPage Messages { get; set; }
    //    /// <summary> Gets or sets mail folders. The user's mail folders. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mailFolders", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserMailFoldersCollectionPage MailFolders { get; set; }
    //    /// <summary> Gets or sets calendar. The user's primary calendar. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "calendar", Required = Newtonsoft.Json.Required.Default)]
    //    public Calendar Calendar { get; set; }
    //    /// <summary> Gets or sets calendars. The user's calendars. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "calendars", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserCalendarsCollectionPage Calendars { get; set; }
    //    /// <summary> Gets or sets calendar groups. The user's calendar groups. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "calendarGroups", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserCalendarGroupsCollectionPage CalendarGroups { get; set; }
    //    /// <summary> Gets or sets calendar view. The calendar view for the calendar. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "calendarView", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserCalendarViewCollectionPage CalendarView { get; set; }
    //    /// <summary> Gets or sets events. The user's events. Default is to show Events under the Default Calendar. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "events", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserEventsCollectionPage Events { get; set; }
    //    /// <summary> Gets or sets people. People that are relevant to the user. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "people", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserPeopleCollectionPage People { get; set; }
    //    /// <summary> Gets or sets contacts. The user's contacts. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "contacts", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserContactsCollectionPage Contacts { get; set; }
    //    /// <summary> Gets or sets contact folders. The user's contacts folders. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "contactFolders", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserContactFoldersCollectionPage ContactFolders { get; set; }
    //    /// <summary> Gets or sets inference classification. Relevance classification of the user's messages based on explicit designations which override inferred relevance or importance.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "inferenceClassification", Required = Newtonsoft.Json.Required.Default)]
    //    public InferenceClassification InferenceClassification { get; set; }
    //    /// <summary> Gets or sets photo. The user's profile photo. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "photo", Required = Newtonsoft.Json.Required.Default)]
    //    public ProfilePhoto Photo { get; set; }
    //    /// <summary> Gets or sets photos.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "photos", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserPhotosCollectionPage Photos { get; set; }
    //    /// <summary> Gets or sets drive. The user's OneDrive. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "drive", Required = Newtonsoft.Json.Required.Default)]
    //    public Drive Drive { get; set; }
    //    /// <summary> Gets or sets drives. A collection of drives available for this user. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "drives", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserDrivesCollectionPage Drives { get; set; }
    //    /// <summary> Gets or sets followed sites.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "followedSites", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserFollowedSitesCollectionWithReferencesPage FollowedSites { get; set; }
    //    /// <summary> Gets or sets extensions. The collection of open extensions defined for the user. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "extensions", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserExtensionsCollectionPage Extensions { get; set; }
    //    /// <summary> Gets or sets managed devices. The managed devices associated with the user.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "managedDevices", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserManagedDevicesCollectionPage ManagedDevices { get; set; }
    //    /// <summary> Gets or sets managed app registrations. Zero or more managed app registrations that belong to the user.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "managedAppRegistrations", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserManagedAppRegistrationsCollectionWithReferencesPage ManagedAppRegistrations { get; set; }
    //    /// <summary> Gets or sets device management troubleshooting events. The list of troubleshooting events for this user.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deviceManagementTroubleshootingEvents", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserDeviceManagementTroubleshootingEventsCollectionPage DeviceManagementTroubleshootingEvents { get; set; }
    //    /// <summary> Gets or sets planner. Entry-point to the Planner resource that might exist for a user. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "planner", Required = Newtonsoft.Json.Required.Default)]
    //    public PlannerUser Planner { get; set; }
    //    /// <summary> Gets or sets insights. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "insights", Required = Newtonsoft.Json.Required.Default)]
    //    public OfficeGraphInsights Insights { get; set; }
    //    /// <summary> Gets or sets settings.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "settings", Required = Newtonsoft.Json.Required.Default)]
    //    public UserSettings Settings { get; set; }
    //    /// <summary> Gets or sets onenote. Read-only.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onenote", Required = Newtonsoft.Json.Required.Default)]
    //    public Onenote Onenote { get; set; }
    //    /// <summary> Gets or sets activities. The user's activities across devices. Read-only. Nullable.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "activities", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserActivitiesCollectionPage Activities { get; set; }
    //    /// <summary> Gets or sets online meetings.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onlineMeetings", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserOnlineMeetingsCollectionPage OnlineMeetings { get; set; }
    //    /// <summary> Gets or sets joined teams.</summary>
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "joinedTeams", Required = Newtonsoft.Json.Required.Default)]
    //    public IUserJoinedTeamsCollectionPage JoinedTeams { get; set; }
    //}
}
