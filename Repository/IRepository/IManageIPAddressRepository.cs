using EntityModels.Models;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IManageIPAddressRepository
    {
        IEnumerable<UserDto> GetUsers();
        IEnumerable<LocationTypeVal> LocationTypeValue();

        IpAdressInfo GetIpInfoaddressid(IpAdressInfo value);
        List<ManageIPAddressViewModal> GetUserInfoId(IpAdressInfo ipAdress);

        List<string> GetUserName(CRUDModel<IpAdressInfo> InfoAddressModel);

        IpAdressInfo GetStartEndIp(string StartIP, string EndIP, int IpAdressInfoID);
        IpAdressInfo SelectForUpdaterange(string StartIP, string EndIP, int IpAdressInfoID);
        void SaveIpAddressInfo(IpAdressInfo ipAdress);
        void SaveUserIpInfo(UserIpInfo userIp);
        IpAdressInfo GetIpAdressInfoID(int IpAdressInfoID);

        void UpdateIpAddressInfo(IpAdressInfo ipAdress);

        List<UserIpInfo> GetUserIpInfo(int iIpaddressId);

        void RemoveIpinfo(CRUDModel<IpAdressInfo> IpAdressInfo);

        List<IpAdressInfoSelect> SelectAddress();

        List<UserActivityList> UserActivityList(int Mode, DateTime dt1, DateTime dt2);

        List<ManageIPAddressViewModal> GetIpAndLocation();

        List<GetUserTrackTime> GetUserwiseAttendance(int UserId, DateTime dt1, DateTime dt2);

        IpAdressInfo GetInLocation(string InLocation);
        void SaveUserTimeTrack(UserTimeTrackInfo usertime);
        void UpdateUserTrackTime(UserTimeTrackInfo usertimetrack);

        UserTimeTrackInfo GetUserTimeTrackInfos(int InTimeId);

        void UpdateIsMailSendAllow(string value);

        void RemoveUserTime(CRUDModel<GetUserTrackTime> Info);

        List<PayPeriodSettings> PayPeriodSettingsList();
        void SavePayPeriodSetting(PayPeriodSettings Obj);
        PayPeriodSettings GetPayPeiodId(int? id);

        void ClearTableWeekMasters();
        void SaveWeekMaster(WeekMaster master);
        void UpdatePayPeriodSetting(PayPeriodSettings Obj);
        List<DrpWeekRange> DrpWeekList();

        decimal GetTimeDuration(int UserId, DateTime date, DateTime date2);
    }
}
