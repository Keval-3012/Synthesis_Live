using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.EJ2.DropDowns;

namespace Repository
{
    public class HREmployeeRepository : IHREmployeeRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HREmployeeRepository(DBContext context)
        {
            _context = context;
        }

        public List<HREmployeeViewModal> GetHREmployeeList(int store_idval,int userid)
        {
            List<HREmployeeViewModal> obj = new List<HREmployeeViewModal>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HREmployeeViewModal>("SP_HREmployee @StoreId = {0},@UserId = {1}", store_idval, userid).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<HREthnicityMaster> GetHREthnicityList()
        {
            List<HREthnicityMaster> obj = new List<HREthnicityMaster>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREthnicityList - " + DateTime.Now);
                obj = _context.HREthnicityMaster.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREthnicityList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeMaster CheckHrEmployeeIfExist(string EmployeeUserName, int EmployeeId)
        {
            HREmployeeMaster obj = new HREmployeeMaster();
            try
            {
                logger.Info("HREmployeeRepository - CheckHrEmployeeIfExist - " + DateTime.Now);
                if (EmployeeId == 0)
                {
                    obj = _context.HREmployeeMaster.Where(s => s.EmployeeUserName.Equals(EmployeeUserName) && s.IsDeleted == false).FirstOrDefault();
                }
                else
                {
                    obj = _context.HREmployeeMaster.Where(s => s.EmployeeUserName.Equals(EmployeeUserName) && s.IsDeleted == false && s.EmployeeId != EmployeeId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - CheckHrEmployeeIfExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public int InsertEmployeeMaster(HREmployeeMaster obj)
        {
            int EmployeeId = 0;
            try
            {
                logger.Info("HREmployeeRepository - InsertEmployeeMaster - " + DateTime.Now);
                EmployeeId = _context.Database.SqlQuery<int>("SP_HRCreateEmployee @Mode = {0},@LoginUserId = {1},@EmployeeUserName = {2},@Password = {3},@ConfirmPassword = {4},@FirstName = {5},@MiddleName = {6}," +
                   "@LastName = {7},@AdditionalLastName = {8},@DateofBirth = {9},@SSN = {10},@Gender = {11},@MaritalStatus = {12},@LanguageId = {13},@EthnicityId = {14},@Email = {15},@Phone = {16},@MobileNo = {17},@Street = {18},@BuildingNo = {19}," +
                   "@City = {20},@State = {21},@ZipCode = {22},@Designation = {23},@FullSSN = {24},@IsTraningCompleted = {25},@UseEmailAsLogin = {26},@IsLocked = {27},@IsDeleted = {28},@IsLanguageSelect = {29},@IsFirstLogin = {30},@CreatedBy = {31},@ModifiedBy = {32},@CreatedOn  = {33},@ModifiedOn  = {34}, @EmployeeID = {35}",
                   "InsertMain", obj.LoginUserId, obj.EmployeeUserName, obj.Password, obj.ConfirmPassword, obj.FirstName,
                   obj.MiddleName, obj.LastName, obj.AdditionalLastName, obj.DateofBirth, obj.SSN, obj.Gender, obj.MaritalStatus, obj.LanguageId, obj.EthnicityId == 0 ? null : obj.EthnicityId
                   , obj.Email, obj.Phone, obj.MobileNo, obj.Street, obj.BuildingNo, obj.City, obj.State, obj.ZipCode, obj.Designation, obj.FullSSN, obj.IsTraningCompleted,
                   obj.UseEmailAsLogin, obj.IsLocked, obj.IsDeleted, obj.IsLanguageSelect, obj.IsFirstLogin, obj.CreatedBy, obj.ModifiedBy, obj.CreatedOn, obj.ModifiedOn, obj.EmployeeId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertEmployeeMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return EmployeeId;
        }

        public int InsertEmployeeChilde(List<HREmployeeChild> obj)
        {
            int EmployeeChildID = 0;
            try
            {
                logger.Info("HREmployeeRepository - InsertEmployeeChilde - " + DateTime.Now);
                foreach (var item in obj)
                {
                    EmployeeChildID = _context.Database.SqlQuery<int>("SP_HRCreateEmployee @Mode = {0},@EmployeeId = {1},@SrNo = {2},@OfficeEmployeeID = {3},@HireDate = {4},@TerminationDate = {5},@Status = {6}," +
                        "@EmployeementTypeStatus = {7},@StoreId = {8},@DepartmentId = {9},@CreatedOn  = {10},@CreatedBy  = {11},@ModifiedOn  = {12},@ModifiedBy  = {13},@EmployeeChildId = {14}",
                        "InsertChild", item.EmployeeId, item.SrNo, item.OfficeEmployeeID, item.HireDate, item.TerminationDate, item.Status, item.EmployeementTypeStatus, item.StoreId,
                        item.DepartmentId, item.CreatedOn, item.CreatedBy, item.ModifiedOn, item.ModifiedBy, item.EmployeeChildId).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertEmployeeChilde - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return EmployeeChildID;
        }

        public HREmployeeMaster GetHREmployeeMaster(int EmployeeId)
        {
            HREmployeeMaster obj = new HREmployeeMaster();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeMaster - " + DateTime.Now);
                obj = _context.HREmployeeMaster.FirstOrDefault(result => result.EmployeeId == EmployeeId);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<HREmployeeChild> GetHREmployeeChild(int EmployeeId)
        {
            List<HREmployeeChild> obj = new List<HREmployeeChild>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeChild - " + DateTime.Now);
                obj = _context.HREmployeeChild.Where(result => result.EmployeeId == EmployeeId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeChild - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public int DeleteChildExtraRecode(int EmployeeId, int? SrNo)
        {
            int Id = 0;
            try
            {
                logger.Info("HREmployeeRepository - DeleteChildExtraRecode - " + DateTime.Now);
                Id = _context.Database.SqlQuery<int>("SP_HRCreateEmployee @Mode = {0}, @EmployeeID = {1}, @SrNo = {2}", "DeleteChild", EmployeeId, SrNo).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - DeleteChildExtraRecode - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Id;
        }

        public bool DeleteEmployee(int EmployeeId, int ModifiedBy)
        {
            try
            {
                logger.Info("HREmployeeRepository - DeleteEmployee - " + DateTime.Now);
                var employeeToDelete = _context.HREmployeeMaster.FirstOrDefault(e => e.EmployeeId == EmployeeId);
                if (employeeToDelete != null)
                {
                    _context.Database.ExecuteSqlCommand("SP_HRCreateEmployee @Mode = {0}, @EmployeeID = {1}, @ModifiedBy = {2}", "Delete", EmployeeId, ModifiedBy);
                    return true; // Return true indicating successful deletion
                }

                return false; // Return false if employee record not found
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - DeleteEmployee - " + DateTime.Now + " - " + ex.Message.ToString());
                // Log the exception or handle it as needed
                return false; // Return false indicating deletion failure due to an exception
            }
        }

        public int GetModuleMastersId(string ModuleName)
        {
            int ID = 0;
            try
            {
                ID = _context.ModuleMasters.Where(s => s.ModuleName == ModuleName).FirstOrDefault().ModuleId;
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetModuleMastersId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ID;
        }
        public List<string> GetUserRoleList_BaasedOnTypeWise(int UserTypeId,int storeId,int ModuleId)
        {
            List<string> userRoles= new List<string>();
            try
            {
                userRoles = _context.userRoles.Where(s => s.UserTypeId == UserTypeId&&s.StoreId==storeId&&s.ModuleId==ModuleId).Select(s=>s.Role).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceRepository - GetModuleMastersId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return userRoles;
        }

        public bool UpdateEmployeePassword(int EmployeeId, int ModifiedBy)
        {
            try
            {
                logger.Info("HREmployeeRepository - UpdateEmployeePassword - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_HRCreateEmployee @Mode = {0}, @EmployeeID = {1}, @ModifiedBy = {2}", "UpdatePassword", EmployeeId, ModifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - UpdateEmployeePassword - " + DateTime.Now + " - " + ex.Message.ToString());
                return false; // Return false indicating deletion failure due to an exception
            }
        }

        public List<StoreDetail> GetEmployeeUsingStore(int EmployeeId)
        {
            List<StoreDetail> sd = new List<StoreDetail>();
            try
            {
                logger.Info("HREmployeeRepository - GetEmployeeUsingStore - " + DateTime.Now);
                sd = _context.Database.SqlQuery<StoreDetail>("SP_HRCreateEmployee @Mode = {0}, @EmployeeID = {1}", "GetStore", EmployeeId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetEmployeeUsingStore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return sd;
        }

        public HREmployeeChild GetHREmployeeChildByEmployeeIdandStoreId(int EmployeeId, int StoreId)
        {
            HREmployeeChild obj = new HREmployeeChild();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeChildByEmployeeIdandStoreId - " + DateTime.Now);
                obj = _context.HREmployeeChild.Where(result => result.EmployeeId == EmployeeId && result.StoreId == StoreId).OrderByDescending(s => s.EmployeeChildId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeChildByEmployeeIdandStoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public string GetSSN(string Password, int EmployeeId,int UserId)
        {
            string SSN = "";
            UserMaster user = new UserMaster();
            try
            {
                logger.Info("HREmployeeRepository - GetSSN - " + DateTime.Now);
                var employee = _context.UserMasters.Where(x => x.UserId == UserId && x.Password == Password).FirstOrDefault();
                if (employee != null)
                {
                    SSN = _context.HREmployeeMaster.Where(s => s.EmployeeId == EmployeeId).FirstOrDefault().FullSSN;
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - GetSSN - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return SSN;
        }

        #region Pay Rate Repository
        public List<HREmployeePayRate> GetHREmployeePayRateList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeePayRate> obj = new List<HREmployeePayRate>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeePayRateList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HREmployeePayRate>("SP_HREmployeePayRate @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetPayRate", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeePayRateList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeePayRate InsertUpdateHREmployeePayRate(HREmployeePayRate obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertUpdateHREmployeePayRate - " + DateTime.Now);
                obj.EmployeePayRateId = _context.Database.SqlQuery<int>("SP_HREmployeePayRate @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeePayRateId = {3}, @PayRateDate = {4}, @PayRate = {5}," +
                    " @PayType = {6}, @Comments = {7}, @CreatedBy = {8}, @CreatedOn = {9}, @ModifiedBy = {10}, @ModifiedOn = {11}, @IsActive = {12}",
                   "InsertAndEdit", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeePayRateId, obj.PayRateDate, obj.PayRate, obj.PayType, obj.Comments, obj.CreatedBy, obj.CreatedOn,
                   obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertUpdateHREmployeePayRate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeePayRate GetHREmployeePayRateByID(int EmployeePayRateId)
        {
            HREmployeePayRate hR = new HREmployeePayRate();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeePayRateByID - " + DateTime.Now);
                hR = _context.HREmployeePayRate.Find(EmployeePayRateId);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeePayRateByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }

        public HREmployeePayRate RemoveHRPayRate(int EmployeePayRateId)
        {
            HREmployeePayRate hr = new HREmployeePayRate();
            try
            {
                logger.Info("HREmployeeRepository - RemoveHRPayRate - " + DateTime.Now);
                hr = _context.HREmployeePayRate.Find(EmployeePayRateId);
                _context.HREmployeePayRate.Remove(hr);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - RemoveHRPayRate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }

        public decimal GetHourlyRate(string Password, int EmployeePayRateId, int UserId)
        {
            decimal payrate = 0;
            UserMaster user = new UserMaster();
            try
            {
                logger.Info("HREmployeeRepository - GetHourlyRate - " + DateTime.Now);
                var employee = _context.UserMasters.Where(x => x.UserId == UserId && x.Password == Password).FirstOrDefault();
                if (employee != null)
                {
                    payrate = Convert.ToDecimal(_context.HREmployeePayRate.Where(s => s.EmployeePayRateId == EmployeePayRateId).FirstOrDefault().PayRate);
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHourlyRate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return payrate;
        }
        #endregion Pay Rate Repository

        #region Sick Time Repository
        public List<HREmployeeSickTimes> GetHREmployeeSickTimeList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeeSickTimes> obj = new List<HREmployeeSickTimes>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeSickTimeList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HREmployeeSickTimes>("SP_HREmployeeSickTime @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetSickTime", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeSickTimeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeSickTimes InsertUpdateHREmployeeSickTimes(HREmployeeSickTimes obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertUpdateHREmployeeSickTimes - " + DateTime.Now);
                obj.EmployeeSickTimeId = _context.Database.SqlQuery<int>("SP_HREmployeeSickTime @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeeSickTimeId = {3}, @EffectiveDate = {4}, @Time = {5}," +
                    " @TimeType = {6}, @Comments = {7}, @CreatedBy = {8}, @CreatedOn = {9}, @ModifiedBy = {10}, @ModifiedOn = {11}, @IsActive = {12}",
                   "InsertAndEdit", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeeSickTimeId, obj.EffectiveDate, obj.Time, obj.TimeType, obj.Comments, obj.CreatedBy, obj.CreatedOn,
                   obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertUpdateHREmployeeSickTimes - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeSickTimes GetHREmployeeSickTimeByID(int EmployeeSickTimeId)
        {
            HREmployeeSickTimes hR = new HREmployeeSickTimes();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeSickTimeByID - " + DateTime.Now);
                hR = _context.HREmployeeSickTimes.FirstOrDefault(s => s.EmployeeSickTimeId == EmployeeSickTimeId);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeSickTimeByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }

        public HREmployeeSickTimes RemoveHREmployeeSickTime(int EmployeeSickTimeId)
        {
            HREmployeeSickTimes hr = new HREmployeeSickTimes();
            try
            {
                logger.Info("HREmployeeRepository - RemoveHREmployeeSickTime - " + DateTime.Now);
                hr = _context.HREmployeeSickTimes.Find(EmployeeSickTimeId);
                _context.HREmployeeSickTimes.Remove(hr);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - RemoveHREmployeeSickTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }
        #endregion Sick Time Repository

        #region Vacation Time Repository
        public List<HREmployeeVacationTime> GetHREmployeeVacationTimeList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeeVacationTime> obj = new List<HREmployeeVacationTime>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeVacationTimeList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HREmployeeVacationTime>("SP_HREmployeeVacationTime @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetVacationTime", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeVacationTimeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeVacationTime InsertUpdateHREmployeeVacationTime(HREmployeeVacationTime obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertUpdateHREmployeeVacationTime - " + DateTime.Now);
                obj.EmployeeVacationTimeId = _context.Database.SqlQuery<int>("SP_HREmployeeVacationTime @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeeVacationTimeId = {3}, @EffectiveDate = {4}, @Time = {5}," +
                    " @TimeType = {6}, @Comments = {7}, @CreatedBy = {8}, @CreatedOn = {9}, @ModifiedBy = {10}, @ModifiedOn = {11}, @IsActive = {12}",
                   "InsertAndEdit", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeeVacationTimeId, obj.EffectiveDate, obj.Time, obj.TimeType, obj.Comments, obj.CreatedBy, obj.CreatedOn,
                   obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertUpdateHREmployeeVacationTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeVacationTime GetHREmployeeVacationByID(int EmployeeVacationTimeId)
        {
            HREmployeeVacationTime hR = new HREmployeeVacationTime();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeVacationByID - " + DateTime.Now);
                hR = _context.HREmployeeVacationTime.FirstOrDefault(s => s.EmployeeVacationTimeId == EmployeeVacationTimeId);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeVacationByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }

        public HREmployeeVacationTime RemoveHREmployeeVacationTime(int EmployeeVacationTimeId)
        {
            HREmployeeVacationTime hr = new HREmployeeVacationTime();
            try
            {
                logger.Info("HREmployeeRepository - RemoveHREmployeeVacationTime - " + DateTime.Now);
                hr = _context.HREmployeeVacationTime.Find(EmployeeVacationTimeId);
                _context.HREmployeeVacationTime.Remove(hr);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - RemoveHREmployeeVacationTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }
        #endregion Vacation Time Repository

        #region Insurance Repsoitory
        public List<HREmployeeInsurance> GetHREmployeeInsurance(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeeInsurance> obj = new List<HREmployeeInsurance>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeInsurance - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HREmployeeInsurance>("SP_HREmployeeInsurance @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetInsuranceDetail", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeInsurance - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeInsurance InsertUpdateHREmployeeInsurance(HREmployeeInsurance obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertUpdateHREmployeeInsurance - " + DateTime.Now);
                obj.EmployeeInsuranceId = _context.Database.SqlQuery<int>("SP_HREmployeeInsurance @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeeInsuranceId = {3}, @EffectiveDate = {4}, @EnrollmentStatus = {5}," +
                    " @FileName = {6}, @Comments = {7}, @IsDeleted = {8}, @CreatedBy = {9}, @CreatedOn = {10}, @ModifiedBy = {11}, @ModifiedOn = {12}, @IsActive = {13}",
                   "InsertAndEdit", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeeInsuranceId, obj.EffectiveDate, obj.EnrollmentStatus, obj.FileName, obj.Comments, obj.IsDeleted, obj.CreatedBy, obj.CreatedOn,
                   obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertUpdateHREmployeeInsurance - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeInsurance RemoveHREmployeeInsurance(int EmployeeInsuranceId)
        {
            HREmployeeInsurance hr = new HREmployeeInsurance();
            try
            {
                logger.Info("HREmployeeRepository - RemoveHREmployeeInsurance - " + DateTime.Now);
                hr = _context.HREmployeeInsurance.Find(EmployeeInsuranceId);
                _context.HREmployeeInsurance.Remove(hr);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - RemoveHREmployeeInsurance - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }

        public HREmployeeInsurance GetHREmployeeInsuranceID(int EmployeeInsuranceId)
        {
            HREmployeeInsurance hR = new HREmployeeInsurance();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeInsuranceID - " + DateTime.Now);
                hR = _context.HREmployeeInsurance.FirstOrDefault(s => s.EmployeeInsuranceId == EmployeeInsuranceId);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeInsuranceID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }
        #endregion Insurance Repository

        #region Documents Repository
        public List<HRFileDetailViewModel> GetHREmployeeDocumentsList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRFileDetailViewModel> obj = new List<HRFileDetailViewModel>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeDocumentsList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRFileDetailViewModel>("SP_HREmployeeDocument @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetDocumentFile", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeDocumentsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeDocument InsertHREmployeeDocuments(HREmployeeDocument obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertHREmployeeDocuments - " + DateTime.Now);
                obj.EmployeeDocumentId = _context.Database.SqlQuery<int>("SP_HREmployeeDocument @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeeDocumentId = {3}, @FileName = {4}," +
                   "@Comments = {5}, @Extension = {6}, @LocationFrom = {7}, @DocumentType = {8}, @CreatedBy = {9}, @CreatedOn = {10}, @ModifiedBy = {11}, @ModifiedOn = {12}, @IsActive = {13}",
                   "InsertDocuments", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeeDocumentId, obj.FileName, obj.Comments, obj.Extension, obj.LocationFrom, obj.DocumentType, obj.CreatedBy, 
                   obj.CreatedOn, obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertHREmployeeDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeDocument RemoveHREmployeeDocument(int DocId, int EmployeeId, string FileName, int ModifiedBy, int Type)
        {
            HREmployeeDocument hr = new HREmployeeDocument();
            try
            {
                logger.Info("HREmployeeRepository - RemoveHREmployeeDocument - " + DateTime.Now);
                if (Type == 1)
                {
                    var obj = _context.HREmployeeRetirementInfo.SingleOrDefault(b => b.EmployeeRetirementInfoId == DocId && b.FileName == FileName && b.EmployeeId == EmployeeId);
                    if (obj != null)
                    {
                        obj.IsActive = false;
                        obj.ModifiedBy = ModifiedBy;
                        obj.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
                else if (Type == 2)
                {
                    var obj = _context.HREmployeeHealthBenefitInfo.SingleOrDefault(b => b.EmployeeHealthBenefitInfoID == DocId && b.DocFileName == FileName && b.EmployeeId == EmployeeId);
                    if (obj != null)
                    {
                        obj.IsActive = false;
                        obj.ModifiedBy = ModifiedBy;
                        obj.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
                else if (Type == 3)
                {
                    var obj = _context.HREmployeeDocument.SingleOrDefault(b => b.EmployeeDocumentId == DocId && b.FileName == FileName && b.EmployeeId == EmployeeId);
                    if (obj != null)
                    {
                        obj.IsActive = false;
                        obj.ModifiedBy = ModifiedBy;
                        obj.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - RemoveHREmployeeDocument - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }

        public HREmployeeRetirementInfo InsertHREmployeeDocuments401K(HREmployeeRetirementInfo obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertHREmployeeDocuments401K - " + DateTime.Now);
                obj.EmployeeRetirementInfoId = _context.Database.SqlQuery<int>("SP_HREmployeeDocument @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeeRetirementInfoId = {3}, @FileName = {4}," +
                   "@OptStatus = {5}, @CreatedBy = {6}, @CreatedOn = {7}, @ModifiedBy = {8}, @ModifiedOn = {9}, @IsActive = {10}",
                   "Insert401KDocument", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeeRetirementInfoId, obj.FileName, obj.OptStatus, obj.CreatedBy,
                   obj.CreatedOn, obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertHREmployeeDocuments401K - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        #endregion Documents Repository

        #region Notes Repository
        public List<HRNoteViewModal> GetHREmployeeNotesList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRNoteViewModal> obj = new List<HRNoteViewModal>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeNotesList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRNoteViewModal>("SP_HREmployeeNotes @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetNotes", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeNotesList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public HREmployeeNotes InsertHREmployeeNotes(HREmployeeNotes obj)
        {
            try
            {
                logger.Info("HREmployeeRepository - InsertHREmployeeNotes - " + DateTime.Now);
                obj.EmployeeNoteId = _context.Database.SqlQuery<int>("SP_HREmployeeNotes @Mode = {0}, @EmployeeId = {1}, @EmployeeChildId = {2}, @EmployeeNoteId = {3}, @Notes = {4}," +
                     "@CreatedBy = {5}, @CreatedOn = {6}, @ModifiedBy = {7}, @ModifiedOn = {8}, @IsActive = {9}",
                   "InsertNotes", obj.EmployeeId, obj.EmployeeChildId, obj.EmployeeNoteId, obj.Notes, obj.CreatedBy, obj.CreatedOn,obj.ModifiedBy, obj.ModifiedOn, obj.IsActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - InsertHREmployeeNotes - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        #endregion Notes Repository

        #region Training History Repsoitory
        public List<HREmployeeMaster> GetHREmployeeTrainingHistory(int EmployeeId, int StoreId)
        {
            List<HREmployeeMaster> obj = new List<HREmployeeMaster>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeTrainingHistory - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HREmployeeMaster>("SP_HREmployeeTrainingHistory @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}", "GetTrainingHistory", EmployeeId, StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeTrainingHistory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        #endregion Training History Repository

        #region Warning Repository
        public List<HRWarningViewModel> GetHREmployeeWarningList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRWarningViewModel> obj = new List<HRWarningViewModel>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeWarningList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRWarningViewModel>("SP_HREmployeeWarning @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetWarningFile", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeWarningList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        #endregion Warning Repository

        #region Termination Repository
        public List<HRTerminationViewModel> GetHREmployeeTerminationList(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRTerminationViewModel> obj = new List<HRTerminationViewModel>();
            try
            {
                logger.Info("HREmployeeRepository - GetHREmployeeTerminationList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRTerminationViewModel>("SP_HREmployeeTermination @Mode = {0}, @EmployeeId = {1}, @StoreId = {2}, @EmployeeChildId = {3}", "GetTerminationFile", EmployeeId, StoreId, EmployeeChildId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeRepository - GetHREmployeeTerminationList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        #endregion Termination Repository
    }
}
