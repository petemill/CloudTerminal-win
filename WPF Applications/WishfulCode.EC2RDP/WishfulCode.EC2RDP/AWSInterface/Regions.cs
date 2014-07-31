using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishfulCode.mRDP.AWSInterface
{
    /// <summary>
    /// Enumeration of all known AWS regions
    /// </summary>
    public enum Region
    {
        US,
        EU,
        SFO,
        APS1,
        APN1
    }


    /// <summary>
    /// Contains static information for regions
    /// </summary>
    public static class RegionHelper
    {
        /// <summary>
        /// Informative name for AWS region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public static string FriendlyNameForRegion(Region region)
        {
            switch (region)
            {
                case Region.US:
                    return "US-East";
                    break;
                case Region.EU:
                    return "EU-West";
                    break;
                case Region.SFO:
                    return "US-West";
                    break;
                case Region.APS1:
                    return "Singapore";
                    break;
                case Region.APN1:
                    return "Japan";
                    break;
                default:
                    return "Unknown Region";
                    break;
            }
        }

        /// <summary>
        /// Endpoint Url for the EC2 API for a given region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public static string EC2EndpointForRegion(Region region)
        {
            switch (region)
            {
                case Region.US:
                    return "http://ec2.us-east-1.amazonaws.com";
                    break;
                case Region.EU:
                    return "http://ec2.eu-west-1.amazonaws.com";
                    break;
                case Region.SFO:
                    return "http://ec2.us-west-1.amazonaws.com";
                    break;
                case Region.APS1:
                    return "http://ec2.ap-southeast-1.amazonaws.com";
                    break;
                case Region.APN1:
                    return "http://ec2.ap-northeast-1.amazonaws.com";
                    break;
                default:
                    return null;
                    break;
            }
        }

        /// <summary>
        /// Endpoint Url for the CloudWatch API for a given region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public static string CloudWatchEndpointForRegion(Region region)
        {
            switch (region)
            {
                case Region.US:
                    return "http://monitoring.us-east-1.amazonaws.com";
                    break;
                case Region.EU:
                    return "http://monitoring.eu-west-1.amazonaws.com";
                    break;
                case Region.SFO:
                    return "http://monitoring.us-west-1.amazonaws.com";
                    break;
                case Region.APS1:
                    return "http://monitoring.ap-southeast-1.amazonaws.com";
                    break;
                case Region.APN1:
                    return "http://monitoring.ap-northeast-1.amazonaws.com";
                    break;
                default:
                    return null;
                    break;
            }
        }

        public static string ELBEndpointForRegion(Region region)
        {
            switch (region)
            {
                case Region.EU:
                    return "https://eu-west-1.elasticloadbalancing.amazonaws.com";
                    break;
                default:
                    throw new Exception("Unhandled region '"+region+"' in ELBEndpointForRegion.");
                    break;
            }
        }

        
    }
}
