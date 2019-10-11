
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;

    /// <summary>
    /// Permissions for authorization requests.
    /// </summary>
    [Flags]
    public enum AuthorizationScope : int
    {
        /// <summary>
        /// Read basic profile (r_basicprofile).
        /// Name, photo, headline, and current positions.
        /// </summary>
        ReadBasicProfile = 0x001,

        /// <summary>
        /// Read full profile (r_fullprofile).
        /// Full profile including experience, education, skills, and recommendations.
        /// </summary>
        ReadFullProfile = 0x002,

        /// <summary>
        /// Read email address (r_emailaddress).
        /// The primary email address you use for your LinkedIn account
        /// </summary>
        ReadEmailAddress = 0x004,

        /// <summary>
        /// Read network (r_network).
        /// Your 1st and 2nd degree connections.
        /// </summary>
        ReadNetwork = 0x008,

        /// <summary>
        /// Read contact information (r_contactinfo).
        /// Address, phone number, and bound accounts.
        /// </summary>
        ReadContactInfo = 0x010,

        /// <summary>
        /// Read write network updates (rw_nus).
        /// Retrieve and post updates to LinkedIn.
        /// </summary>
        ReadWriteNetworkUpdates = 0x020,

        /// <summary>
        /// Read write company page (rw_company_admin).
        /// Edit company pages for which I am an Admin and post status updates on behalf of those companies.
        /// </summary>
        ReadWriteCompanyPage = 0x040,

        /// <summary>
        /// Read write groups (rw_groups).
        /// Retrieve and post group discussions.
        /// </summary>
        ReadWriteGroups = 0x080,

        /// <summary>
        /// Write messages (w_messages).
        /// Send messages and invitations to connect.
        /// </summary>
        WriteMessages = 0x100,

        /// <summary>
        /// Share (w_share).
        /// Post a comment that includes a URL to the content you wish to share.
        /// Share with specific values — You provide the title, description, image, etc.
        /// </summary>
        /// <remarks>
        /// https://developer.linkedin.com/docs/share-on-linkedin
        /// </remarks>
        WriteShare = 0x200,

        /// <summary>
        /// Read write company page (rw_organization).
        /// Edit company pages for which I am an Admin and post status updates on behalf of those companies.
        /// </summary>
        ReadWriteOrganization = 0x400,

        /// <summary>
        /// Reade First Connections Size(r_1st_connections_size)
        /// Read access to the number of 1st-degreeconnections within the authenticated member's network
        /// </summary>
        ReadFirstConnectionsSize = 0x800,

        /// <summary>
        /// Read Lite Profile (r_liteprofile)
        /// Read access to a member's lite profile. This permission scope permits access to retrieve the member's ID via the Profile API. Alternatively, retrieve the member ID from any of the LinkedIn Profile APIs.
        /// </summary>
        ReadLiteProfile = 0x1000,

        /// <summary>
        /// Read Ads Reporting (r_ads_reporting)
        /// </summary>
        ReadAdsReporting = 0x2000,

        /// <summary>
        /// Retrieve organizations' posts, comments, and likes 
        /// </summary>
        ReadOrganizationSocial = 0x4000,

        /// <summary>
        /// Retrieve member’s posts, comments, likes, and other engagement data.
        /// </summary>
        ReadMemberSocial = 0x8000,

        /// <summary>
        /// Post, comment and like posts on member’s behalf
        /// </summary>
        WriteMemberSocial = 0x10000,

        /// <summary>
        /// Manage member’s organizations' pages and retrieve reporting data
        /// </summary>
        ReadWriteOrganizationAdmin = 0x20000,

        /// <summary>
        /// Post, comment and like posts on behalf of member’s organizations
        /// </summary>
        WriteOrganizationSocial = 0x40000,
        
        /// <summary>
        /// Read organizations by ids
        /// </summary>
        ReadOrganizationLookup = 0x80000
    }
}
