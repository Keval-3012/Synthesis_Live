using EntityModels.Models;
using NLog;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ManageIPAddressRepository : IManageIPAddressRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public ManageIPAddressRepository(DBContext context)
        {
            _context = context;
        }

        public IEnumerable<UserDto> GetUsers()
        {
            IEnumerable<UserDto> users = new List<UserDto>();

            try
            {
                users = from p in _context.UserMasters
                        where p.TrackHours.Equals(true)
                        select new UserDto { UserId = p.UserId, UserName = p.UserName };
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUsers - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return users;
        }

        public IEnumerable<LocationTypeVal> LocationTypeValue()
        {
            IEnumerable<LocationTypeVal> locationvalue = new List<LocationTypeVal>();

            try
            {
                locationvalue = from LocationType e in Enum.GetValues(typeof(LocationType))
                                    select new LocationTypeVal
                                    {
                                        ID = (int)e,
                                        Name = e.ToString()
                                    };
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - LocationTypeValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return locationvalue;
        }

        public IpAdressInfo GetIpInfoaddressid(IpAdressInfo value)
        {
            IpAdressInfo IpInfoaddressid = new IpAdressInfo();
            try
            {
                IpInfoaddressid = _context.IpAdressInfos.Where(s => s.IpAdressInfoID == value.IpAdressInfoID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetIpInfoaddressid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return IpInfoaddressid;
        }

        public List<ManageIPAddressViewModal> GetUserInfoId(IpAdressInfo ipAdress)
        {
            List<ManageIPAddressViewModal> obj = new List<ManageIPAddressViewModal>();
            try
            {
                obj = _context.UserIpInfos.Where(s => s.IpAdressInfoID == ipAdress.IpAdressInfoID).Select(s => new ManageIPAddressViewModal { UserId = s.UserID }).ToList();


            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUserInfoId - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return obj;
        }

        public List<string> GetUserName(CRUDModel<IpAdressInfo>InfoAddressModel)
        {
            List<string> obj = new List<string>();
            try
            {
                var Data = InfoAddressModel.Value.MultiUserId.Select(s => s).ToList();
                obj = _context.UserMasters.Where(s => Data.Contains(s.UserId.ToString())).Select(s => s.UserName ).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUserName - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return obj;
        }

        public IpAdressInfo GetStartEndIp(string StartIP, string EndIP, int IpAdressInfoID)
        {
            IpAdressInfo obj = new IpAdressInfo();
            try
            {
                obj = _context.Database.SqlQuery<IpAdressInfo>("SP_IpAddressInfo @Mode = {0},@StartIP = {1},@EndIP={2},@IpAdressInfoID={3}", "SelectForUpdateStaticIp", StartIP, EndIP, IpAdressInfoID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetStartEndIp - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public IpAdressInfo SelectForUpdaterange(string StartIP, string EndIP, int IpAdressInfoID)
        {
            IpAdressInfo obj = new IpAdressInfo();
            try
            {
                obj = _context.Database.SqlQuery<IpAdressInfo>("SP_IpAddressInfo @Mode = {0},@StartIP = {1},@EndIP={2},@IpAdressInfoID={3}", "SelectForUpdateRange", StartIP, EndIP, IpAdressInfoID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SelectForUpdaterange - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SaveIpAddressInfo(IpAdressInfo ipAdress)
        {
            try
            {
                _context.IpAdressInfos.Add(ipAdress);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SaveIpAddressInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveUserIpInfo(UserIpInfo userIp)
        {
            try
            {
                _context.UserIpInfos.Add(userIp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SaveUserIpInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public IpAdressInfo GetIpAdressInfoID(int IpAdressInfoID)
        {
            IpAdressInfo obj = new IpAdressInfo();
            try
            {
                obj = _context.IpAdressInfos.Where(x => x.IpAdressInfoID == IpAdressInfoID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetIpAdressInfoID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void UpdateIpAddressInfo(IpAdressInfo ipAdress)
        {
            try
            {
                _context.Entry(ipAdress).State = EntityState.Modified;
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - UpdateIpAddressInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        public List<UserIpInfo> GetUserIpInfo(int iIpaddressId)
        {
            List<UserIpInfo> obj = new List<UserIpInfo>();
            try
            {
                obj = _context.UserIpInfos.Where(s => s.IpAdressInfoID == iIpaddressId).ToList();
                _context.UserIpInfos.RemoveRange(obj);
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUserIpInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return obj;
        }

        public void RemoveIpinfo(CRUDModel<IpAdressInfo> IpAdressInfo)
        {
            IpAdressInfo obj = new IpAdressInfo();
            try
            {
                if (IpAdressInfo.Deleted == null)
                {
                    obj = _context.IpAdressInfos.Find(IpAdressInfo.Key);
                    _context.IpAdressInfos.Remove(obj);
                }
                else
                {
                    foreach (var item in IpAdressInfo.Deleted)
                    {
                        obj = _context.IpAdressInfos.Find(item.IpAdressInfoID);
                        _context.IpAdressInfos.Remove(obj);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - RemoveIpinfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<IpAdressInfoSelect> SelectAddress()
        {
            List<IpAdressInfoSelect> obj = new List<IpAdressInfoSelect>();
            try
            {
                obj = _context.Database.SqlQuery<IpAdressInfoSelect>("SP_IpAddressInfo @Mode = {0}", "SelectIpAddressInfo").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SelectAddress - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<UserActivityList> UserActivityList(int Mode, DateTime dt1, DateTime dt2)
        {
            List<UserActivityList> obj = new List<UserActivityList>();

            try
            {
               obj = _context.Database.SqlQuery<UserActivityList>("SP_Attandanace @Mode = {0}, @UserId = {1}, @startDate = {2}, @endDate = {3}", "GetUserTimeList", 0, dt1, dt2).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - UserActivityList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<ManageIPAddressViewModal> GetIpAndLocation()
        {
            List<ManageIPAddressViewModal> obj = new List<ManageIPAddressViewModal>();
            try
            {
                obj = _context.IpAdressInfos.Select(s => new ManageIPAddressViewModal { Location = s.Location, IpAdressInfoID = s.IpAdressInfoID }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetIpAndLocation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<GetUserTrackTime> GetUserwiseAttendance(int UserId, DateTime dt1, DateTime dt2)
        {
            List<GetUserTrackTime> obj = new List<GetUserTrackTime>();

            try
            {
                obj = _context.Database.SqlQuery<GetUserTrackTime>("SP_Attandanace @Mode = {0}, @UserId = {1},@startDate = {2}, @endDate = {3}", "GetUserWiseAttandanace", UserId, dt1, dt2).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUserwiseAttendance - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public IpAdressInfo GetInLocation(string InLocation)
        {
            IpAdressInfo obj = new IpAdressInfo();
            try
            {
                obj = _context.IpAdressInfos.Where(s => s.Location == InLocation).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetInLocation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SaveUserTimeTrack(UserTimeTrackInfo usertime)
        {
            try
            {
                _context.UserTimeTrackInfos.Add(usertime);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SaveUserTimeTrack - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateUserTrackTime(UserTimeTrackInfo usertimetrack)
        {
            try
            {
                _context.Entry(usertimetrack).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - UpdateUserTrackTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        public UserTimeTrackInfo GetUserTimeTrackInfos(int InTimeId)
        {
            UserTimeTrackInfo obj = new UserTimeTrackInfo();
            try
            {
                obj = _context.UserTimeTrackInfos.Where(s => s.UserTimeTrackInfoID == InTimeId).FirstOrDefault();
                _context.UserTimeTrackInfos.Remove(obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUserTimeTrackInfos - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return obj;
            
        }

        public void UpdateIsMailSendAllow(string value)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("[SP_PayPeriodSettings] @Mode={0},@IsMailSendAllow={1}", "UpdateIsMailSendAllow", value); //.Replace("&amp;","&").Replace("&apos;","'")
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - UpdateIsMailSendAllow - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

        public UserTimeTrackInfo GetUserTimeKey(object infokey)
        {
            UserTimeTrackInfo obj = new UserTimeTrackInfo();
            try
            {
                obj = _context.UserTimeTrackInfos.Find(infokey);
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetUserTimeKey - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public UserTimeTrackInfo DeleteUserInOutTime(object infokey, int UserId, string IpAddress, string InOutType)
        {
            UserTimeTrackInfo obj = new UserTimeTrackInfo();
            try
            {
                obj = _context.Database.SqlQuery<UserTimeTrackInfo>("SP_Attandanace @Mode = {0}, @InTimeIds = {1},@UserId = {2},@IpAddress = {3},@InOutTypes = {4}", "DeleteUserInOutTime", infokey, UserId, IpAddress, InOutType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - DeleteUserInOutTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void RemoveUserTime(CRUDModel<GetUserTrackTime> Info)
        {
            try
            {
                if (Info.Deleted == null)
                {

                    UserTimeTrackInfo modelvalue = _context.UserTimeTrackInfos.Find(Info.Key);

                    UserTimeTrackInfo value = _context.Database.SqlQuery<UserTimeTrackInfo>("SP_Attandanace @Mode = {0}, @InTimeIds = {1},@UserId = {2},@IpAddress = {3},@InOutTypes = {4}", "DeleteUserInOutTime", Info.Key, modelvalue.UserId, modelvalue.IpAddress, modelvalue.InOutType).FirstOrDefault();
                    if (value == null)
                    {
                        var list = _context.UserTimeTrackInfos.Where(s => s.UserTimeTrackInfoID == modelvalue.UserTimeTrackInfoID).ToList();
                        _context.UserTimeTrackInfos.RemoveRange(list);
                    }
                    else
                    {
                        var list = _context.UserTimeTrackInfos.Where(s => s.UserTimeTrackInfoID == modelvalue.UserTimeTrackInfoID || s.UserTimeTrackInfoID == value.UserTimeTrackInfoID).ToList();
                        _context.UserTimeTrackInfos.RemoveRange(list);
                    }

                }
                else
                {
                    foreach (var item in Info.Deleted)
                    {
                        var dt = _context.UserTimeTrackInfos.Where(s => s.UserTimeTrackInfoID == item.InTimeId || s.UserTimeTrackInfoID == item.OutTimeId).ToList();
                        _context.UserTimeTrackInfos.RemoveRange(dt);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - RemoveUserTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<PayPeriodSettings> PayPeriodSettingsList()
        {
            List<PayPeriodSettings> obj = new List<PayPeriodSettings>();
            try
            {
                obj = _context.PayPeriodSettings.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - PayPeriodSettingsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SavePayPeriodSetting(PayPeriodSettings Obj)
        {
            try
            {
                _context.PayPeriodSettings.Add(Obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SavePayPeriodSetting - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public PayPeriodSettings GetPayPeiodId(int? id)
        {
            PayPeriodSettings obj = new PayPeriodSettings();
            try
            {
                obj = _context.PayPeriodSettings.Find(id);
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetPayPeiodId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void ClearTableWeekMasters()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("[SP_IpAddressInfo] @Mode={0}", "ClearTableWeekMasters"); //.Replace("&amp;","&").Replace("&apos;","'")
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - ClearTableWeekMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveWeekMaster(WeekMaster master)
        {
            try
            {
                _context.WeekMasters.Add(master);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - SaveWeekMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdatePayPeriodSetting(PayPeriodSettings Obj)
        {
            try
            {
                _context.Entry(Obj).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - UpdatePayPeriodSetting - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<DrpWeekRange> DrpWeekList()
        {
            List<DrpWeekRange> obj = new List<DrpWeekRange>();
            try
            {
                obj = _context.Database.SqlQuery<DrpWeekRange>("SP_IpAddressInfo @Mode = {0}", "GetWeekMastersData").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - DrpWeekList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public decimal GetTimeDuration(int UserId, DateTime date, DateTime date2)
        {
            decimal duration = 0;
            try
            {
                duration = _context.Database.SqlQuery<decimal>("SP_Attandanace @Mode = {0}, @UserId = {1},@startDate = {2}, @endDate = {3}", "GetTimeDuration", UserId, date, date2).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressRepository - GetTimeDuration - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return duration;
        }

    }
}



