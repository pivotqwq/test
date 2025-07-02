using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using static backend.Data.ApplicationDbContext;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataSeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 清空所有表数据
        [HttpPost("clear-all")]
        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                // 按依赖顺序删除数据
                _context.ClinicalDatas.RemoveRange(_context.ClinicalDatas);
                _context.ProteinDatas.RemoveRange(_context.ProteinDatas);
                _context.GenomicDatas.RemoveRange(_context.GenomicDatas);
                _context.SpecimenQualities.RemoveRange(_context.SpecimenQualities);
                _context.SpecimenInfos.RemoveRange(_context.SpecimenInfos);
                _context.QuestionnaireDatas.RemoveRange(_context.QuestionnaireDatas);
                _context.IndividualHealthBehaviors.RemoveRange(_context.IndividualHealthBehaviors);
                _context.HouseholdEnvironments.RemoveRange(_context.HouseholdEnvironments);
                _context.RegionalEnvironments.RemoveRange(_context.RegionalEnvironments);
                _context.PulmonaryDetails.RemoveRange(_context.PulmonaryDetails);
                _context.ImagingDetails.RemoveRange(_context.ImagingDetails);
                _context.LabTests.RemoveRange(_context.LabTests);
                _context.MedicalCosts.RemoveRange(_context.MedicalCosts);
                _context.FollowUpRecords.RemoveRange(_context.FollowUpRecords);
                _context.MedicationRecords.RemoveRange(_context.MedicationRecords);
                _context.PhysicalExaminations.RemoveRange(_context.PhysicalExaminations);
                _context.Diagnoses.RemoveRange(_context.Diagnoses);
                _context.PatientStaffRelations.RemoveRange(_context.PatientStaffRelations);
                _context.FamilyHistories.RemoveRange(_context.FamilyHistories);
                _context.MedicalHistories.RemoveRange(_context.MedicalHistories);
                _context.Contacts.RemoveRange(_context.Contacts);
                _context.Insurance.RemoveRange(_context.Insurance);
                _context.PatientBasicInfos.RemoveRange(_context.PatientBasicInfos);
                _context.InvestigatorQualifications.RemoveRange(_context.InvestigatorQualifications);
                _context.Admins.RemoveRange(_context.Admins);
                // Patients表已删除，无需清理
                _context.Users.RemoveRange(_context.Users);
                _context.Memos.RemoveRange(_context.Memos);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "所有表数据已清空"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "清空数据失败: " + ex.Message 
                });
            }
        }

        // 只插入用户（测试用）
        [HttpPost("seed-users-only")]
        public async Task<IActionResult> SeedUsersOnly()
        {
            try
            {
                var result = await SeedUsers();
                return Ok(new { 
                    success = true, 
                    message = "用户数据插入完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "用户数据插入失败: " + ex.Message + " | InnerException: " + (ex.InnerException?.Message ?? "None")
                });
            }
        }

        // 测试单个研究员记录插入
        [HttpPost("test-investigator")]
        public async Task<IActionResult> TestInvestigatorInsert()
        {
            try
            {
                var investigator = new InvestigatorQualification 
                { 
                    investigator_id = "TEST001", 
                    name = "测试研究员", 
                    qualification = "测试资质", 
                    institution = "测试机构", 
                    position = "测试职位", 
                    contact_phone = "12345678900", 
                    create_time = DateTime.UtcNow
                };

                _context.InvestigatorQualifications.Add(investigator);
                await _context.SaveChangesAsync();
                
                return Ok(new { 
                    success = true, 
                    message = "测试研究员插入成功",
                    details = "插入了1条InvestigatorQualification记录"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "测试研究员插入失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 只插入基础数据（用户、患者、管理员等）
        [HttpPost("seed-basic")]
        public async Task<IActionResult> SeedBasicData()
        {
            try
            {
                var results = new List<string>();
                results.Add(await SeedUsers());
                // SeedPatients已删除，不再需要
                results.Add(await SeedAdmins());
                results.Add(await SeedInvestigatorQualifications());
                results.Add(await SeedPatientBasicInfos());

                return Ok(new { 
                    success = true, 
                    message = "基础数据插入完成",
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "基础数据插入失败: " + ex.Message + " | InnerException: " + (ex.InnerException?.Message ?? "None")
                });
            }
        }

        // 只插入临床数据
        [HttpPost("seed-clinical")]
        public async Task<IActionResult> SeedClinicalDataOnly()
        {
            try
            {
                var results = new List<string>();
                results.Add(await SeedDiagnoses());
                results.Add(await SeedPhysicalExaminations());
                results.Add(await SeedMedicationRecords());
                results.Add(await SeedFollowUpRecords());
                results.Add(await SeedMedicalCosts());
                results.Add(await SeedLabTests());

                return Ok(new { 
                    success = true, 
                    message = "临床数据插入完成",
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "临床数据插入失败: " + ex.Message 
                });
            }
        }

        // 只插入研究数据（样本、基因组、蛋白质等）
        [HttpPost("seed-research")]
        public async Task<IActionResult> SeedResearchData()
        {
            try
            {
                var results = new List<string>();
                results.Add(await SeedSpecimenInfos());
                results.Add(await SeedSpecimenQualities());
                results.Add(await SeedGenomicData());
                results.Add(await SeedProteinData());
                results.Add(await SeedClinicalData());

                return Ok(new { 
                    success = true, 
                    message = "研究数据插入完成",
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "研究数据插入失败: " + ex.Message 
                });
            }
        }

        // 为所有表插入测试数据
        [HttpPost("seed-all")]
        public async Task<IActionResult> SeedAllData()
        {
            try
            {
                var results = new List<string>();

                // 1. 先插入基础数据
                results.Add(await SeedUsers());
                // SeedPatients已删除，不再需要
                results.Add(await SeedAdmins());
                results.Add(await SeedInvestigatorQualifications());

                // 2. 插入患者相关数据
                results.Add(await SeedPatientBasicInfos());
                results.Add(await SeedInsurance());
                results.Add(await SeedContacts());
                results.Add(await SeedMedicalHistory());
                results.Add(await SeedFamilyHistory());
                results.Add(await SeedPatientStaffRelations());

                // 3. 插入临床数据
                results.Add(await SeedDiagnoses());
                results.Add(await SeedPhysicalExaminations());
                results.Add(await SeedMedicationRecords());
                results.Add(await SeedFollowUpRecords());
                results.Add(await SeedMedicalCosts());

                // 4. 插入检查数据
                results.Add(await SeedLabTests());
                results.Add(await SeedImagingDetails());
                results.Add(await SeedPulmonaryDetails());

                // 5. 插入环境和行为数据
                results.Add(await SeedRegionalEnvironments());
                results.Add(await SeedHouseholdEnvironments());
                results.Add(await SeedIndividualHealthBehaviors());
                results.Add(await SeedQuestionnaireData());

                // 6. 插入样本和实验数据
                results.Add(await SeedSpecimenInfos());
                results.Add(await SeedSpecimenQualities());
                results.Add(await SeedGenomicData());
                results.Add(await SeedProteinData());
                results.Add(await SeedClinicalData());

                // 7. 插入其他数据
                results.Add(await SeedMemos());

                return Ok(new { 
                    success = true, 
                    message = "所有表数据种子插入完成，每个患者都有完整的关联数据，所有患者都分配给管理员用户",
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "数据种子插入失败: " + ex.Message 
                });
            }
        }

        // 生成完整的患者关联数据（确保每个患者都有所有类型的数据）
        [HttpPost("seed-complete-patient-data")]
        public async Task<IActionResult> SeedCompletePatientData()
        {
            try
            {
                var results = new List<string>();
                var summary = new List<string>();

                // 首先确保有基础用户和管理员数据
                if (!await _context.Users.AnyAsync())
                {
                    results.Add(await SeedUsers());
                    results.Add(await SeedAdmins());
                }

                // 确保有调研员数据
                if (!await _context.InvestigatorQualifications.AnyAsync())
                {
                    results.Add(await SeedInvestigatorQualifications());
                }

                // 核心患者数据 - 每个患者都必须有的数据
                var coreResults = new List<string>();
                // SeedPatients已删除，只使用PatientBasicInfo表
                coreResults.Add(await SeedPatientBasicInfos()); // 生成详细基本信息
                coreResults.Add(await SeedInsurance());
                coreResults.Add(await SeedContacts());
                coreResults.Add(await SeedMedicalHistory());
                coreResults.Add(await SeedFamilyHistory());
                coreResults.Add(await SeedPatientStaffRelations()); // 所有患者分配给管理员

                // 临床数据 - 每个患者的医疗记录
                var clinicalResults = new List<string>();
                clinicalResults.Add(await SeedDiagnoses());
                clinicalResults.Add(await SeedPhysicalExaminations());
                clinicalResults.Add(await SeedMedicationRecords());
                clinicalResults.Add(await SeedFollowUpRecords());
                clinicalResults.Add(await SeedMedicalCosts());

                // 检查数据 - 每个患者的检查记录
                var examResults = new List<string>();
                examResults.Add(await SeedLabTests());
                examResults.Add(await SeedImagingDetails());
                examResults.Add(await SeedPulmonaryDetails());

                // 调研数据 - 每个患者的环境和行为数据
                var researchResults = new List<string>();
                researchResults.Add(await SeedRegionalEnvironments());
                researchResults.Add(await SeedHouseholdEnvironments());
                researchResults.Add(await SeedIndividualHealthBehaviors());
                researchResults.Add(await SeedQuestionnaireData());

                // 实验数据 - 每个患者的样本和基因数据
                var labResults = new List<string>();
                labResults.Add(await SeedSpecimenInfos());
                labResults.Add(await SeedSpecimenQualities());
                labResults.Add(await SeedGenomicData());
                labResults.Add(await SeedProteinData());
                labResults.Add(await SeedClinicalData());

                // 其他数据
                var otherResults = new List<string>();
                otherResults.Add(await SeedMemos());

                results.AddRange(coreResults);
                results.AddRange(clinicalResults);
                results.AddRange(examResults);
                results.AddRange(researchResults);
                results.AddRange(labResults);
                results.AddRange(otherResults);

                // 生成数据关联摘要
                summary.Add("✅ 核心患者数据：基本信息、保险、联系人、病史、家族史");
                summary.Add("✅ 患者-医生关系：所有患者都分配给管理员用户作为主治医师");
                summary.Add("✅ 临床数据：诊断、体检、用药、随访、费用记录");
                summary.Add("✅ 检查数据：实验室检查、影像学检查、肺功能检查");
                summary.Add("✅ 调研数据：区域环境、家庭环境、个人行为、问卷数据");
                summary.Add("✅ 实验数据：标本信息、质量控制、基因组数据、蛋白质数据");
                summary.Add("✅ 每个患者(P001-P020)都有完整的9类关联数据");
                summary.Add("✅ 数据关联关系：patient_id作为主键关联所有患者相关表");

                var patientCount = await _context.PatientBasicInfos.CountAsync();
                var followupCount = await _context.FollowUpRecords.CountAsync();
                var specimenCount = await _context.SpecimenInfos.CountAsync();

                return Ok(new { 
                    success = true, 
                    message = $"完整患者关联数据生成完成！共创建{patientCount}个患者的完整数据集，包含{followupCount}条随访记录和{specimenCount}个标本记录",
                    summary = summary,
                    details = results,
                    dataStats = new 
                    {
                        totalPatients = patientCount,
                        followupRecords = followupCount,
                        specimenRecords = specimenCount,
                        dataCategories = 9
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "完整患者关联数据生成失败: " + ex.Message 
                });
            }
        }

        // 测试单个诊断记录插入
        [HttpPost("test-diagnoses")]
        public async Task<IActionResult> TestDiagnosesInsert()
        {
            try
            {
                var result = await SeedDiagnoses();
                return Ok(new { 
                    success = true, 
                    message = "诊断数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "诊断数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试医保信息插入
        [HttpPost("test-insurance")]
        public async Task<IActionResult> TestInsuranceInsert()
        {
            try
            {
                var result = await SeedInsurance();
                return Ok(new { 
                    success = true, 
                    message = "医保数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "医保数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试联系人信息插入
        [HttpPost("test-contacts")]
        public async Task<IActionResult> TestContactsInsert()
        {
            try
            {
                var result = await SeedContacts();
                return Ok(new { 
                    success = true, 
                    message = "联系人数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "联系人数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试环境数据插入
        [HttpPost("test-environments")]
        public async Task<IActionResult> TestEnvironmentsInsert()
        {
            try
            {
                var results = new List<string>();
                results.Add(await SeedRegionalEnvironments());
                results.Add(await SeedHouseholdEnvironments());
                results.Add(await SeedIndividualHealthBehaviors());
                results.Add(await SeedQuestionnaireData());

                return Ok(new { 
                    success = true, 
                    message = "环境数据插入测试完成",
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "环境数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试区域环境数据插入
        [HttpPost("test-regional-environments")]
        public async Task<IActionResult> TestRegionalEnvironmentsInsert()
        {
            try
            {
                var result = await SeedRegionalEnvironments();
                return Ok(new { 
                    success = true, 
                    message = "区域环境数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "区域环境数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试家庭环境数据插入
        [HttpPost("test-household-environments")]
        public async Task<IActionResult> TestHouseholdEnvironmentsInsert()
        {
            try
            {
                var result = await SeedHouseholdEnvironments();
                return Ok(new { 
                    success = true, 
                    message = "家庭环境数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "家庭环境数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试个人健康行为数据插入
        [HttpPost("test-individual-health-behaviors")]
        public async Task<IActionResult> TestIndividualHealthBehaviorsInsert()
        {
            try
            {
                var result = await SeedIndividualHealthBehaviors();
                return Ok(new { 
                    success = true, 
                    message = "个人健康行为数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "个人健康行为数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // 测试问卷数据插入
        [HttpPost("test-questionnaire-data")]
        public async Task<IActionResult> TestQuestionnaireDataInsert()
        {
            try
            {
                var result = await SeedQuestionnaireData();
                return Ok(new { 
                    success = true, 
                    message = "问卷数据插入测试完成",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "问卷数据插入测试失败: " + ex.Message,
                    innerException = ex.InnerException?.Message ?? "None",
                    stackTrace = ex.StackTrace
                });
            }
        }

        private async Task<string> SeedUsers()
        {
            if (await _context.Users.AnyAsync()) return "Users表已有数据，跳过";

            var users = new List<User>();
            var professions = new[] { "主治医师", "副主任医师", "主任医师", "住院医师", "护士", "主管护师", "护师", "研究员", "副研究员", "主任研究员", "技师", "主管技师", "药师", "主管药师", "检验师", "影像医师", "康复师", "营养师", "心理医师", "全科医师" };
            var surnames = new[] { "张", "王", "李", "赵", "刘", "陈", "杨", "黄", "周", "吴", "徐", "孙", "马", "朱", "胡", "林", "郭", "何", "高", "罗" };
            var givenNames = new[] { "伟", "芳", "娜", "敏", "静", "丽", "强", "磊", "军", "洋", "勇", "艳", "杰", "涛", "明", "超", "秀英", "桂英", "秀兰", "玉兰" };

            for (int i = 1; i <= 20; i++)
            {
                var surname = surnames[(i - 1) % surnames.Length];
                var givenName = givenNames[(i - 1) % givenNames.Length];
                var profession = professions[(i - 1) % professions.Length];
                var name = surname + givenName;
                
                users.Add(new User 
                { 
                    Id = Guid.NewGuid().ToString(), 
                    username = $"user{i:D3}", 
                    password = "123456", 
                    email = $"user{i:D3}@hospital.com", 
                    name = name, 
                    profession = profession, 
                    phone = $"138{i:D8}" 
                });
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
            return $"Users表插入{users.Count}条数据";
        }

        // SeedPatients方法已删除 - 现在使用PatientBasicInfo表

        private async Task<string> SeedAdmins()
        {
            if (await _context.Admins.AnyAsync()) return "Admins表已有数据，跳过";

            // 获取已插入的用户ID
            var doctorUser = await _context.Users.FirstOrDefaultAsync(u => u.username == "doctor1");
            var researcherUser = await _context.Users.FirstOrDefaultAsync(u => u.username == "researcher1");
            var admin2024User = await _context.Users.FirstOrDefaultAsync(u => u.username == "admin2024");
            var admin2025User = await _context.Users.FirstOrDefaultAsync(u => u.username == "admin2025");

            var admins = new List<Admin>();

            if (doctorUser != null)
            {
                admins.Add(new Admin { user_id = doctorUser.Id, is_admin = true });
            }

            if (researcherUser != null)
            {
                admins.Add(new Admin { user_id = researcherUser.Id, is_admin = true });
            }

            if (admin2024User != null)
            {
                admins.Add(new Admin { user_id = admin2024User.Id, is_admin = true });
            }

            if (admin2025User != null)
            {
                admins.Add(new Admin { user_id = admin2025User.Id, is_admin = true });
            }

            if (admins.Count == 0)
            {
                return "Admin插入失败：找不到相关用户";
            }

            _context.Admins.AddRange(admins);
            await _context.SaveChangesAsync();
            return $"Admins表插入{admins.Count}条数据";
        }

        [HttpPost("set-admin2024")]
        public async Task<IActionResult> SetAdmin2024()
        {
            try
            {
                // 查找admin2024用户
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.username == "admin2024");
                if (adminUser == null)
                {
                    return BadRequest(new { success = false, message = "未找到admin2024用户" });
                }

                // 检查是否已经是管理员
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.user_id == adminUser.Id);
                if (existingAdmin != null)
                {
                    existingAdmin.is_admin = true;
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "admin2024管理员权限更新成功" });
                }

                // 添加新的管理员记录
                var newAdmin = new Admin
                {
                    user_id = adminUser.Id,
                    is_admin = true
                };

                _context.Admins.Add(newAdmin);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "admin2024管理员权限设置成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"设置管理员权限失败: {ex.Message}" });
            }
        }

        [HttpPost("create-and-set-admin2025")]
        public async Task<IActionResult> CreateAndSetAdmin2025()
        {
            try
            {
                // 检查admin2025用户是否已存在
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.username == "admin2025");
                bool userCreated = false;
                
                if (existingUser == null)
                {
                    // 创建admin2025用户
                    var newUser = new User
                    {
                        username = "admin2025",
                        password = "102300326",
                        email = "admin2025@example.com",
                        name = "系统管理员",
                        profession = "管理员"
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    existingUser = newUser;
                    userCreated = true;
                }

                // 强制设置管理员权限
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.user_id == existingUser.Id);
                bool adminCreated = false;
                
                if (existingAdmin != null)
                {
                    existingAdmin.is_admin = true;
                }
                else
                {
                    var newAdmin = new Admin
                    {
                        user_id = existingUser.Id,
                        is_admin = true
                    };
                    _context.Admins.Add(newAdmin);
                    adminCreated = true;
                }

                await _context.SaveChangesAsync();

                string message = userCreated 
                    ? "admin2025账号创建成功并设置为管理员"
                    : "admin2025账号已存在，管理员权限已更新";

                return Ok(new { 
                    success = true, 
                    message = message,
                    userId = existingUser.Id,
                    username = "admin2025",
                    password = "102300326",
                    userCreated = userCreated,
                    adminCreated = adminCreated
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"创建管理员账号失败: {ex.Message}" });
            }
        }

        [HttpPost("force-set-admin/{username}")]
        public async Task<IActionResult> ForceSetAdmin(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.username == username);
                if (user == null)
                {
                    return BadRequest(new { success = false, message = $"用户 {username} 不存在" });
                }

                // 删除现有的管理员记录（如果存在）
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.user_id == user.Id);
                if (existingAdmin != null)
                {
                    _context.Admins.Remove(existingAdmin);
                }

                // 添加新的管理员记录
                var newAdmin = new Admin
                {
                    user_id = user.Id,
                    is_admin = true
                };
                _context.Admins.Add(newAdmin);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = $"用户 {username} 已强制设置为管理员",
                    userId = user.Id,
                    username = username
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"设置管理员权限失败: {ex.Message}" });
            }
        }

        private async Task<string> SeedInvestigatorQualifications()
        {
            if (await _context.InvestigatorQualifications.AnyAsync()) return "InvestigatorQualifications表已有数据，跳过";

            var qualifications = new List<InvestigatorQualification>();
            var surnames = new[] { "张", "王", "李", "赵", "刘", "陈", "杨", "黄", "周", "吴", "徐", "孙", "马", "朱", "胡", "林", "郭", "何", "高", "罗" };
            var titles = new[] { "医生", "研究员", "教授", "主任", "副主任", "专家", "学者", "博士", "主治医师", "副教授", "讲师", "助理研究员", "高级研究员", "首席专家", "学科带头人", "院士", "特聘教授", "客座教授", "兼职教授", "顾问" };
            var qualificationLevels = new[] { "初级研究员", "中级研究员", "高级研究员", "特级教授", "主任医师", "副主任医师", "主治医师", "住院医师", "博士后", "博士", "硕士", "学士", "高级职称", "中级职称", "初级职称", "特聘专家", "客座专家", "荣誉教授", "资深专家", "首席科学家" };
            var institutions = new[] { "北京儿童医院", "上海儿科研究所", "广州医科大学", "复旦大学附属儿科医院", "首都医科大学附属北京儿童医院", "中南大学湘雅二医院", "华中科技大学同济医学院", "四川大学华西医院", "西安交通大学第一附属医院", "南京医科大学第一附属医院", "浙江大学医学院附属儿童医院", "中山大学孙逸仙纪念医院", "天津医科大学总医院", "大连医科大学附属第一医院", "哈尔滨医科大学附属第二医院", "吉林大学第一医院", "重庆医科大学附属儿童医院", "昆明医科大学第一附属医院", "新疆医科大学第一附属医院", "内蒙古医科大学附属医院" };
            var positions = new[] { "呼吸内科主任", "过敏免疫研究员", "儿科学教授", "小儿呼吸科副主任", "免疫学研究组长", "过敏反应科主任", "儿童哮喘专科医师", "临床免疫学专家", "儿科呼吸病学教授", "小儿过敏科主任医师", "呼吸系统疾病研究员", "儿童免疫缺陷病专家", "小儿肺科主任", "过敏性疾病诊疗专家", "儿科重症医学科主任", "小儿感染免疫科医师", "呼吸康复科主任", "儿童哮喘防治中心主任", "小儿呼吸内镜专家", "儿科临床药理学专家" };

            for (int i = 1; i <= 20; i++)
            {
                var surname = surnames[(i - 1) % surnames.Length];
                var title = titles[(i - 1) % titles.Length];
                var name = surname + title;
                var qualification = qualificationLevels[(i - 1) % qualificationLevels.Length];
                var institution = institutions[(i - 1) % institutions.Length];
                var position = positions[(i - 1) % positions.Length];

                qualifications.Add(new InvestigatorQualification 
                { 
                    investigator_id = $"INV{i:D3}", 
                    name = name, 
                    qualification = qualification, 
                    institution = institution, 
                    position = position, 
                    contact_phone = $"138{i:D8}", 
                    create_time = DateTime.UtcNow 
                });
            }

            _context.InvestigatorQualifications.AddRange(qualifications);
            await _context.SaveChangesAsync();
            return $"InvestigatorQualifications表插入{qualifications.Count}条数据";
        }

        private async Task<string> SeedPatientBasicInfos()
        {
            if (await _context.PatientBasicInfos.AnyAsync()) return "PatientBasicInfos表已有数据，跳过";

            var basicInfos = new List<PatientBasicInfo>();
            var maleNames = new[] { "小明", "小刚", "小华", "小强", "小军", "小伟", "小峰", "小涛", "小龙", "小虎", "小鹏", "小宇", "小凯", "小斌", "小磊", "小辉", "小杰", "小勇", "小飞", "小东" };
            var femaleNames = new[] { "小红", "小丽", "小芳", "小燕", "小娟", "小霞", "小敏", "小静", "小婷", "小雯", "小琳", "小蓉", "小倩", "小慧", "小萍", "小艳", "小玲", "小欣", "小颖", "小莉" };
            var allergyTypes = new[] { "花粉过敏", "尘螨过敏", "食物过敏", "药物过敏", "无明显过敏史", "动物毛发过敏", "化学物质过敏", "紫外线过敏", "冷热过敏", "接触性过敏", "霉菌过敏", "真菌过敏", "草类过敏", "树木过敏", "昆虫叮咬过敏", "金属过敏", "化妆品过敏", "洗涤用品过敏", "防腐剂过敏", "人工色素过敏" };
            var residenceTypes = new[] { "1", "2", "3" }; // 1-城市 2-城镇 3-农村

            for (int i = 1; i <= 20; i++)
            {
                var isMale = i % 2 == 1;
                var gender = isMale ? "M" : "F";
                var name = isMale ? maleNames[(i - 1) / 2 % maleNames.Length] : femaleNames[(i - 1) / 2 % femaleNames.Length];
                var birthYear = 2014 + (i % 6); // 2014-2019年出生
                var birthMonth = (i % 12) + 1;
                var birthDay = (i % 28) + 1;
                var birthDate = DateTime.SpecifyKind(new DateTime(birthYear, birthMonth, birthDay), DateTimeKind.Utc);
                var ageAtDiagnosis = 1.0m + (i % 50) / 10.0m; // 1.0-6.0岁
                var residenceType = residenceTypes[(i - 1) % residenceTypes.Length];
                var allergyHistory = allergyTypes[(i - 1) % allergyTypes.Length];

                basicInfos.Add(new PatientBasicInfo 
                { 
                    patient_id = $"P{i:D3}", 
                    name = name, 
                    gender = gender, 
                    birth_date = birthDate, 
                    age_at_diagnosi = ageAtDiagnosis, 
                    residence_type = residenceType, 
                    allergy_history = allergyHistory, 
                    create_time = DateTime.UtcNow 
                });
            }

            _context.PatientBasicInfos.AddRange(basicInfos);
            await _context.SaveChangesAsync();
            return $"PatientBasicInfos表插入{basicInfos.Count}条数据";
        }

        private async Task<string> SeedInsurance()
        {
            if (await _context.Insurance.AnyAsync()) return "Insurance表已有数据，跳过";

            var insurances = new List<Insurance>();
            var insuranceTypes = new[] { "城镇居民医保", "城镇职工医保", "新农合", "商业保险", "公费医疗", "大病保险", "补充医疗保险", "学生平安保险", "儿童医保", "重疾险", "意外伤害保险", "住院医疗保险", "门诊医疗保险", "特殊疾病保险", "慢性病保险", "康复医疗保险", "药物保险", "器械保险", "护理保险", "健康保险" };

            for (int i = 1; i <= 20; i++)
            {
                var insuranceType = insuranceTypes[(i - 1) % insuranceTypes.Length];

                insurances.Add(new Insurance 
                { 
                    insurance_id = $"INS{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    insurance_type = insuranceType 
                });
            }

            _context.Insurance.AddRange(insurances);
            await _context.SaveChangesAsync();
            return $"Insurance表插入{insurances.Count}条数据";
        }

        private async Task<string> SeedContacts()
        {
            if (await _context.Contacts.AnyAsync()) return "Contacts表已有数据，跳过";

            var contacts = new List<Contact>();
            var relationships = new[] { "妈妈", "爸爸", "奶奶", "爷爷", "外婆", "外公", "阿姨", "叔叔", "舅舅", "姑妈", "监护人", "继父", "继母", "养父", "养母", "保姆", "其他亲属", "朋友", "邻居", "老师" };
            var maleNames = new[] { "小明", "小刚", "小华", "小强", "小军", "小伟", "小峰", "小涛", "小龙", "小虎", "小鹏", "小宇", "小凯", "小斌", "小磊", "小辉", "小杰", "小勇", "小飞", "小东" };
            var femaleNames = new[] { "小红", "小丽", "小芳", "小燕", "小娟", "小霞", "小敏", "小静", "小婷", "小雯", "小琳", "小蓉", "小倩", "小慧", "小萍", "小艳", "小玲", "小欣", "小颖", "小莉" };

            for (int i = 1; i <= 20; i++)
            {
                var isMale = i % 2 == 1;
                var patientName = isMale ? maleNames[(i - 1) / 2 % maleNames.Length] : femaleNames[(i - 1) / 2 % femaleNames.Length];
                var relationship = relationships[(i - 1) % relationships.Length];
                var contactName = patientName + relationship;

                contacts.Add(new Contact 
                { 
                    contact_id = $"C{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    name = contactName, 
                    contact_info = $"137{i:D8}" 
                });
            }

            _context.Contacts.AddRange(contacts);
            await _context.SaveChangesAsync();
            return $"Contacts表插入{contacts.Count}条数据";
        }

        private async Task<string> SeedMedicalHistory()
        {
            if (await _context.MedicalHistories.AnyAsync()) return "MedicalHistory表已有数据，跳过";

            var histories = new List<MedicalHistory>();
            var allergyHistories = new[] 
            { 
                "3岁时出现花粉过敏，每年春季发作",
                "2岁时诊断尘螨过敏，夜间症状明显",
                "食物过敏，主要对鸡蛋、牛奶过敏",
                "青霉素过敏史",
                "无明显过敏史，偶有上呼吸道感染",
                "接触性皮炎，对金属过敏",
                "药物过敏史，对头孢类抗生素过敏",
                "季节性过敏性鼻炎，秋季加重",
                "动物毛发过敏，接触猫狗后出现症状",
                "化学物质过敏，对洗涤剂敏感",
                "日光性皮炎，阳光暴晒后起疹",
                "冷热过敏，温度变化时出现荨麻疹",
                "霉菌过敏，在潮湿环境中症状加重",
                "昆虫叮咬过敏，蚊虫叮咬后局部肿胀",
                "海鲜过敏，食用虾蟹后出现过敏反应",
                "坚果过敏，花生核桃等引起过敏",
                "香料过敏，接触香水化妆品后不适",
                "橡胶过敏，接触乳胶制品后过敏",
                "染发剂过敏，使用染发产品后头皮红肿",
                "防腐剂过敏，食用含防腐剂食品后不适"
            };

            for (int i = 1; i <= 20; i++)
            {
                var allergyHistory = allergyHistories[(i - 1) % allergyHistories.Length];

                histories.Add(new MedicalHistory 
                { 
                    history_id = $"MH{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    allergy_history = allergyHistory 
                });
            }

            _context.MedicalHistories.AddRange(histories);
            await _context.SaveChangesAsync();
            return $"MedicalHistory表插入{histories.Count}条数据";
        }

        private async Task<string> SeedFamilyHistory()
        {
            if (await _context.FamilyHistories.AnyAsync()) return "FamilyHistory表已有数据，跳过";

            var familyHistories = new List<FamilyHistory>();
            var familyAllergyHistories = new[] 
            { 
                "母亲有哮喘病史，父亲有过敏性鼻炎",
                "外婆有过敏性皮炎，母亲有哮喘",
                "父亲有食物过敏史",
                "家族无明显过敏史",
                "爷爷有慢性支气管炎",
                "奶奶有过敏性结膜炎，母亲有湿疹",
                "父亲有药物过敏史，叔叔有哮喘",
                "外公有季节性过敏，姨妈有荨麻疹",
                "母亲有接触性皮炎，舅舅有过敏性咳嗽",
                "父亲有花粉症，爷爷有慢性鼻炎",
                "奶奶有食物不耐受，母亲有过敏性胃炎",
                "外婆有化学物质过敏，父亲有职业性哮喘",
                "母亲有金属过敏，哥哥有过敏性紫癜",
                "父亲有日光过敏，姐姐有特应性皮炎",
                "爷爷有慢性荨麻疹，奶奶有过敏性休克史",
                "母亲有乳胶过敏，父亲有昆虫过敏",
                "外公有海鲜过敏，外婆有坚果过敏",
                "父亲有香料过敏，母亲有防腐剂不耐受",
                "奶奶有染料过敏，爷爷有粉尘过敏",
                "家族多人有过敏体质，易发过敏性疾病"
            };

            for (int i = 1; i <= 20; i++)
            {
                var familyAllergyHistory = familyAllergyHistories[(i - 1) % familyAllergyHistories.Length];

                familyHistories.Add(new FamilyHistory 
                { 
                    family_history_id = $"FH{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    allergy_history = familyAllergyHistory 
                });
            }

            _context.FamilyHistories.AddRange(familyHistories);
            await _context.SaveChangesAsync();
            return $"FamilyHistory表插入{familyHistories.Count}条数据";
        }

        private async Task<string> SeedPatientStaffRelations()
        {
            if (await _context.PatientStaffRelations.AnyAsync()) return "PatientStaffRelations表已有数据，跳过";

            // 获取管理员用户ID，优先使用admin2025，其次admin2024，最后使用第一个用户
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.username == "admin2025") ??
                           await _context.Users.FirstOrDefaultAsync(u => u.username == "admin2024") ??
                           await _context.Users.FirstAsync();
            
            if (adminUser == null)
            {
                return "PatientStaffRelations表插入失败：未找到管理员用户记录";
            }

            var relations = new List<PatientStaffRelation>();
            var relationTypes = new[] { "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师", "主治医师" };

            for (int i = 1; i <= 20; i++)
            {
                var relationType = "主治医师"; // 所有患者都分配主治医师关系
                var startDate = DateTime.UtcNow.AddMonths(-(i % 12 + 1)); // 1-12个月前开始

                relations.Add(new PatientStaffRelation 
                { 
                    patient_id = $"P{i:D3}", 
                    staff_id = adminUser.Id, // 所有患者都分配给管理员用户
                    relation_type = relationType, 
                    start_date = startDate 
                });
            }

            _context.PatientStaffRelations.AddRange(relations);
            await _context.SaveChangesAsync();
            return $"PatientStaffRelations表插入{relations.Count}条数据，所有患者都分配给管理员用户: {adminUser.username} (ID: {adminUser.Id})";
        }

        private async Task<string> SeedDiagnoses()
        {
            if (await _context.Diagnoses.AnyAsync()) return "Diagnoses表已有数据，跳过";

            var diagnoses = new List<Diagnosis>();
            var diseaseNames = new[] { "过敏性哮喘", "过敏性鼻炎", "食物过敏", "药物过敏", "过敏性皮炎", "过敏性结膜炎", "荨麻疹", "特应性皮炎", "过敏性紫癜", "血管神经性水肿", "接触性皮炎", "日光性皮炎", "季节性过敏", "职业性过敏", "过敏性胃肠炎", "过敏性支气管炎", "慢性荨麻疹", "急性过敏反应", "过敏性休克", "多系统过敏综合征" };
            var severityLevels = new[] { "轻度", "中度", "重度", "极重度" };
            var descriptions = new[] 
            { 
                "季节性哮喘，主要由花粉引起",
                "常年性过敏性鼻炎，尘螨过敏",
                "多种食物过敏，需严格忌口",
                "青霉素类药物过敏",
                "接触性皮炎，症状较轻",
                "过敏性结膜炎，双眼红肿流泪",
                "急性荨麻疹，全身风团样皮疹",
                "特应性皮炎，皮肤干燥瘙痒",
                "过敏性紫癜，下肢散在出血点",
                "血管神经性水肿，面部肿胀明显",
                "接触性皮炎，局部红肿糜烂",
                "日光性皮炎，日晒后皮肤红斑",
                "季节性过敏，春秋两季发作",
                "职业性过敏，工作环境相关",
                "过敏性胃肠炎，腹痛腹泻症状",
                "过敏性支气管炎，慢性咳嗽咳痰",
                "慢性荨麻疹，反复发作超过6周",
                "急性过敏反应，需要紧急处理",
                "过敏性休克，危及生命需抢救",
                "多系统过敏综合征，累及多个器官"
            };

            for (int i = 1; i <= 20; i++)
            {
                var diseaseName = diseaseNames[(i - 1) % diseaseNames.Length];
                var severity = severityLevels[(i - 1) % severityLevels.Length];
                var description = descriptions[(i - 1) % descriptions.Length];

                diagnoses.Add(new Diagnosis 
                { 
                    diagnosis_id = $"D{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    disease_name = diseaseName, 
                    severity = severity, 
                    description = description 
                });
            }

            _context.Diagnoses.AddRange(diagnoses);
            await _context.SaveChangesAsync();
            return $"Diagnoses表插入{diagnoses.Count}条数据";
        }

        private async Task<string> SeedPhysicalExaminations()
        {
            if (await _context.PhysicalExaminations.AnyAsync()) return "PhysicalExaminations表已有数据，跳过";

            var examinations = new List<PhysicalExamination>();
            var lungSounds = new[] { "双肺呼吸音清晰", "双肺可闻及轻微哮鸣音", "呼吸音粗糙", "双肺可闻及湿性啰音", "左肺呼吸音减弱", "右肺可闻及干性啰音", "双肺呼吸音对称", "双肺底可闻及细湿啰音", "呼吸音清晰无异常", "双肺可闻及散在哮鸣音", "左下肺呼吸音粗糙", "双肺可闻及高调哮鸣音", "呼吸音正常", "右上肺呼吸音减弱", "双肺可闻及粗湿啰音", "左肺可闻及胸膜摩擦音", "双肺呼吸音清晰对称", "右肺底可闻及细啰音", "呼吸音略粗糙", "双肺可闻及中等哮鸣音" };
            var rashDescriptions = new[] { "无皮疹", "鼻翼两侧轻度红肿", "面部散在红疹", "无异常", "手臂轻微皮疹", "颈部可见抓痕", "双手背部红斑", "腹部散在丘疹", "四肢可见风团", "面颊部轻度红肿", "胸前散在红点", "背部可见湿疹样改变", "双腿膝盖周围红疹", "手腕部位红斑", "眼睑轻度水肿", "耳后可见红疹", "肘窝部位皮炎", "腰部散在红斑", "足背部轻度红肿", "全身皮肤无异常" };

            for (int i = 1; i <= 20; i++)
            {
                var temperature = 36.2m + (i % 15) * 0.1m; // 36.2-37.6度
                var pulse = 75 + (i % 30); // 75-104次/分
                var oxygenSaturation = 95 + (i % 5); // 95-99%
                var lungSound = lungSounds[(i - 1) % lungSounds.Length];
                var rashDescription = rashDescriptions[(i - 1) % rashDescriptions.Length];
                var examDate = DateTime.UtcNow.AddDays(-(i * 2)); // 每隔2天检查一次

                examinations.Add(new PhysicalExamination 
                { 
                    patient_id = $"P{i:D3}", 
                    exam_date = examDate, 
                    temperature = temperature, 
                    pulse = pulse, 
                    oxygen_saturation = oxygenSaturation, 
                    lung_sounds = lungSound, 
                    rash_description = rashDescription 
                });
            }

            _context.PhysicalExaminations.AddRange(examinations);
            await _context.SaveChangesAsync();
            return $"PhysicalExaminations表插入{examinations.Count}条数据";
        }

        private async Task<string> SeedMedicationRecords()
        {
            if (await _context.MedicationRecords.AnyAsync()) return "MedicationRecords表已有数据，跳过";

            var medications = new List<MedicationRecord>();
            var drugNames = new[] { "布地奈德吸入剂", "孟鲁司特钠", "西替利嗪", "氯雷他定", "炉甘石洗剂", "地塞米松", "泼尼松", "扑尔敏", "苯海拉明", "异丙嗪", "茶苯海明", "盐酸羟嗪", "酮替芬", "色甘酸钠", "咪唑斯汀", "非索非那定", "左西替利嗪", "盐酸西替利嗪", "苯磺酸氨氯地平", "盐酸二甲双胍" };
            var dosages = new[] { "200μg", "4mg", "5mg", "10mg", "适量", "0.75mg", "5mg", "4mg", "25mg", "25mg", "25mg", "25mg", "1mg", "20mg", "10mg", "60mg", "5mg", "10mg", "5mg", "500mg" };
            var frequencies = new[] { "每日2次", "每日1次", "每日1次", "每日1次", "每日3次外用", "每日1次", "每日1次", "每日3次", "每日2次", "每日1次", "每日2次", "每日1次", "每日2次", "每日3次", "每日1次", "每日1次", "每日1次", "每日1次", "每日1次", "每日2次" };
            var drugCategories = new[] { "吸入性糖皮质激素", "白三烯受体拮抗剂", "抗组胺药", "抗组胺药", "外用药", "糖皮质激素", "糖皮质激素", "抗组胺药", "抗组胺药", "抗组胺药", "抗组胺药", "抗组胺药", "抗过敏药", "抗过敏药", "抗组胺药", "抗组胺药", "抗组胺药", "抗组胺药", "降压药", "降糖药" };

            for (int i = 1; i <= 20; i++)
            {
                var drugName = drugNames[(i - 1) % drugNames.Length];
                var dosage = dosages[(i - 1) % dosages.Length];
                var frequency = frequencies[(i - 1) % frequencies.Length];
                var drugCategory = drugCategories[(i - 1) % drugCategories.Length];
                var startDate = DateTime.UtcNow.AddDays(-(i * 3)); // 每隔3天开始用药

                medications.Add(new MedicationRecord 
                { 
                    patient_id = $"P{i:D3}", 
                    drug_name = drugName, 
                    dosage = dosage, 
                    frequency = frequency, 
                    start_date = startDate, 
                    drug_category = drugCategory 
                });
            }

            _context.MedicationRecords.AddRange(medications);
            await _context.SaveChangesAsync();
            return $"MedicationRecords表插入{medications.Count}条数据";
        }

        private async Task<string> SeedFollowUpRecords()
        {
            if (await _context.FollowUpRecords.AnyAsync()) return "FollowUpRecords表已有数据，跳过";

            var followUps = new List<FollowUpRecord>();
            var symptomImprovements = new[] 
            { 
                "症状明显改善，夜间咳嗽减少",
                "鼻塞症状有所缓解",
                "皮疹范围缩小",
                "过敏反应消失",
                "皮疹基本消退",
                "过敏性结膜炎眼部症状减轻",
                "荨麻疹发作频次显著降低",
                "特应性皮炎瘙痒明显缓解",
                "过敏性紫癜出血点逐渐吸收",
                "血管神经性水肿肿胀消退",
                "接触性皮炎局部红肿好转",
                "日光性皮炎避光后症状改善",
                "季节性过敏症状得到控制",
                "职业性过敏脱离环境后好转",
                "过敏性胃肠炎腹痛缓解",
                "过敏性支气管炎咳嗽明显减少",
                "慢性荨麻疹复发间隔延长",
                "急性过敏反应得到有效控制",
                "过敏性休克后生命体征稳定",
                "多系统过敏各项指标趋于正常"
            };
            var adverseEffects = new[] 
            { 
                "无不良反应", "轻微嗜睡", "无", "无", "皮肤轻微干燥", "偶有口干", "轻度头晕", "无明显不适", "食欲略减", "轻微恶心", "偶有乏力", "皮肤略显干燥", "无特殊不适", "轻度便秘", "偶有头痛", "睡眠质量略差", "无明显副作用", "轻微腹胀", "偶有心悸", "无不良反应"
            };

            for (int i = 1; i <= 20; i++)
            {
                var symptomImprovement = symptomImprovements[(i - 1) % symptomImprovements.Length];
                var adverseEffect = adverseEffects[(i - 1) % adverseEffects.Length];
                var actScore = 10 + (i % 16); // ACT评分 10-25分
                var followupDate = DateTime.UtcNow.AddDays(-(i + 3)); // 3天前开始随访

                followUps.Add(new FollowUpRecord 
                { 
                    patient_id = $"P{i:D3}", 
                    followup_date = followupDate, 
                    symptom_improvement = symptomImprovement, 
                    adverse_effects = adverseEffect, 
                    act_score = actScore 
                });
            }

            _context.FollowUpRecords.AddRange(followUps);
            await _context.SaveChangesAsync();
            return $"FollowUpRecords表插入{followUps.Count}条数据";
        }

        private async Task<string> SeedMedicalCosts()
        {
            if (await _context.MedicalCosts.AnyAsync()) return "MedicalCosts表已有数据，跳过";

            var costs = new List<MedicalCost>();
            var costTypes = new[] { "门诊费", "药费", "检查费", "治疗费", "住院费", "手术费", "化验费", "影像费", "材料费", "护理费", "床位费", "诊疗费", "康复费", "输液费", "急诊费", "专家费", "会诊费", "监护费", "麻醉费", "器械费" };
            var costAmounts = new[] { 150.00m, 280.50m, 320.00m, 450.00m, 89.50m, 120.00m, 560.00m, 180.00m, 220.00m, 95.00m, 75.00m, 380.00m, 420.00m, 160.00m, 200.00m, 300.00m, 250.00m, 800.00m, 650.00m, 110.00m };

            for (int i = 1; i <= 20; i++)
            {
                var costType = costTypes[(i - 1) % costTypes.Length];
                var amount = costAmounts[(i - 1) % costAmounts.Length];
                var costDate = DateTime.UtcNow.AddDays(-(i * 2 + 10)); // 从12天前开始

                costs.Add(new MedicalCost 
                { 
                    patient_id = $"P{i:D3}", 
                    cost_type = costType, 
                    amount = amount, 
                    cost_date = costDate 
                });
            }

            _context.MedicalCosts.AddRange(costs);
            await _context.SaveChangesAsync();
            return $"MedicalCosts表插入{costs.Count}条数据";
        }

        private async Task<string> SeedLabTests()
        {
            if (await _context.LabTests.AnyAsync()) return "LabTests表已有数据，跳过";

            var labTests = new List<LabTest>();
            var itemNames = new[] { "血常规", "IgE总量", "胸部X线", "肺功能检查", "过敏原检测", "皮肤点刺试验", "嗜酸性粒细胞计数", "特异性IgE", "胸部CT", "FeNO检测", "维生素D检测", "C反应蛋白", "血沉", "肝功能", "肾功能", "电解质", "血气分析", "痰培养", "鼻拭子培养", "过敏原皮试" };
            var examValues = new[] 
            { 
                "白细胞计数：8.5×10^9/L，正常",
                "总IgE：450 IU/mL，偏高",
                "双肺纹理增粗",
                "FEV1/FVC：75%，轻度受限",
                "尘螨++，花粉+",
                "多项过敏原阳性",
                "嗜酸性粒细胞：8%，偏高",
                "牛奶特异性IgE：3.2 kU/L，阳性",
                "双肺散在磨玻璃影",
                "FeNO：35 ppb，轻度升高",
                "25-OH维生素D：20 ng/mL，不足",
                "CRP：5.2 mg/L，轻度升高",
                "血沉：15 mm/h，正常",
                "ALT：25 U/L，正常",
                "肌酐：45 μmol/L，正常",
                "钠：140 mmol/L，正常",
                "pH：7.38，正常",
                "未见致病菌生长",
                "金黄色葡萄球菌阳性",
                "屋尘螨++，花粉+，霉菌+"
            };
            var examTypes = new[] { "blood", "blood", "imaging", "pulmonary", "allergy", "allergy", "blood", "blood", "imaging", "pulmonary", "blood", "blood", "blood", "blood", "blood", "blood", "blood", "microbiology", "microbiology", "allergy" };

            for (int i = 1; i <= 20; i++)
            {
                var itemName = itemNames[(i - 1) % itemNames.Length];
                var examValue = examValues[(i - 1) % examValues.Length];
                var examType = examTypes[(i - 1) % examTypes.Length];

                labTests.Add(new LabTest 
                { 
                    lab_id = $"L{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    item_name = itemName, 
                    exam_value = examValue, 
                    exam_type = examType 
                });
            }

            _context.LabTests.AddRange(labTests);
            await _context.SaveChangesAsync();
            return $"LabTests表插入{labTests.Count}条数据";
        }

        private async Task<string> SeedImagingDetails()
        {
            if (await _context.ImagingDetails.AnyAsync()) return "ImagingDetails表已有数据，跳过";

            var imagingDetails = new List<ImagingDetail>();
            var examDetails = new[] 
            { 
                "胸部X线显示：双肺纹理增粗，未见明显实质性病变，心影大小正常",
                "胸部CT显示：双肺散在小结节影，考虑炎症性改变",
                "头颅MRI显示：脑实质未见异常信号，鼻窦炎症改变",
                "腹部B超显示：肝脾肾未见异常，胆囊壁略厚",
                "心脏彩超显示：各房室结构正常，心功能正常",
                "胸部高分辨CT显示：双肺底轻度纤维化，支气管壁增厚",
                "鼻窦CT显示：双侧上颌窦、筛窦炎症，鼻中隔偏曲",
                "四肢X线显示：骨质密度正常，关节间隙对称",
                "颈椎X线显示：生理曲度存在，椎体边缘略毛糙",
                "胸椎MRI显示：椎体信号正常，椎间盘轻度退变",
                "腰椎CT显示：L4-5椎间盘突出，神经根受压",
                "肺部PET-CT显示：右上肺代谢活跃结节，SUV值2.8",
                "全身骨扫描显示：多发骨转移可能，需进一步检查",
                "冠脉CTA显示：前降支轻度狭窄，余血管通畅",
                "脑血管造影显示：椎基底动脉轻度迂曲，血流通畅",
                "胃镜显示：胃窦部轻度糜烂性胃炎，幽门螺杆菌阳性",
                "肠镜显示：结肠多发息肉，建议内镜下切除",
                "支气管镜显示：气管黏膜充血水肿，少量分泌物",
                "关节MRI显示：膝关节积液，半月板撕裂",
                "甲状腺彩超显示：甲状腺结节，边界清楚，血流信号丰富"
            };

            for (int i = 1; i <= 20; i++)
            {
                var examDetail = examDetails[(i - 1) % examDetails.Length];

                imagingDetails.Add(new ImagingDetail 
                { 
                    imaging_id = $"IMG{i:D3}", 
                    lab_id = $"L{i:D3}", 
                    exam_details = examDetail 
                });
            }

            _context.ImagingDetails.AddRange(imagingDetails);
            await _context.SaveChangesAsync();
            return $"ImagingDetails表插入{imagingDetails.Count}条数据";
        }

        private async Task<string> SeedPulmonaryDetails()
        {
            if (await _context.PulmonaryDetails.AnyAsync()) return "PulmonaryDetails表已有数据，跳过";

            var pulmonaryDetails = new List<PulmonaryDetail>();
            var examDetails = new[] 
            { 
                "肺功能检查详情：FVC 85% 预测值，FEV1 75% 预测值，FEV1/FVC 75%，提示轻度阻塞性通气功能障碍",
                "FeNO检测：45 ppb，提示气道炎症水平中等",
                "激发试验：甲胆碱激发试验阳性，PC20 0.8 mg/ml，提示气道高反应性",
                "六分钟步行试验：步行距离480米，氧饱和度下降2%，轻度活动耐量受限",
                "痰诱导检查：嗜酸性粒细胞比例8%，中性粒细胞85%，提示气道炎症",
                "肺弥散功能：DLCO 78% 预测值，弥散功能轻度下降",
                "呼吸肌力测定：最大吸气压90 cmH2O，最大呼气压120 cmH2O，正常范围",
                "脉冲震荡肺功能：R5 0.45 kPa·s/L，X5 -0.18 kPa·s/L，小气道阻力增加",
                "运动肺功能试验：运动后FEV1下降15%，运动诱发性支气管痉挛阳性",
                "血气分析：pH 7.42，PO2 88 mmHg，PCO2 38 mmHg，轻度低氧血症",
                "支气管舒张试验：沙丁胺醇吸入后FEV1改善12%，支气管舒张试验阳性",
                "夜间血氧监测：最低血氧饱和度89%，平均血氧饱和度94%",
                "呼出气冷凝液检测：pH 6.8，8-异前列腺素浓度升高",
                "肺容量测定：总肺容量110% 预测值，残气容量130% 预测值",
                "胸膜功能检查：胸膜腔压力-8 cmH2O，胸膜弹性正常",
                "咳嗽反射敏感性：辣椒素C5浓度0.98 μmol/L，咳嗽反射增强",
                "睡眠呼吸监测：AHI 15次/小时，轻度睡眠呼吸暂停",
                "高原适应性检查：海拔2500米模拟，血氧饱和度下降至85%",
                "呼吸训练评估：腹式呼吸训练后肺活量提高8%",
                "气道阻力测定：总气道阻力2.8 cmH2O·s/L，气道阻力轻度增加"
            };

            for (int i = 1; i <= 20; i++)
            {
                var examDetail = examDetails[(i - 1) % examDetails.Length];

                pulmonaryDetails.Add(new PulmonaryDetail 
                { 
                    pulmonary_id = $"PUL{i:D3}", 
                    lab_id = $"L{i:D3}", 
                    exam_details = examDetail 
                });
            }

            _context.PulmonaryDetails.AddRange(pulmonaryDetails);
            await _context.SaveChangesAsync();
            return $"PulmonaryDetails表插入{pulmonaryDetails.Count}条数据";
        }

        private async Task<string> SeedRegionalEnvironments()
        {
            if (await _context.RegionalEnvironments.AnyAsync()) return "RegionalEnvironments表已有数据，跳过";

            var environments = new List<RegionalEnvironment>();
            var regionNames = new[] { "北京市朝阳区", "上海市浦东区", "广州市天河区", "深圳市南山区", "杭州市西湖区", "南京市鼓楼区", "武汉市江汉区", "成都市锦江区", "西安市雁塔区", "重庆市渝中区", "天津市和平区", "青岛市市南区", "大连市中山区", "厦门市思明区", "苏州市工业园区", "宁波市鄞州区", "无锡市滨湖区", "长沙市岳麓区", "郑州市金水区", "济南市历下区" };
            var climateTypes = new[] { "温带大陆性", "亚热带季风", "亚热带季风", "亚热带海洋性", "亚热带季风", "温带季风", "亚热带季风", "亚热带湿润", "温带大陆性", "亚热带湿润", "温带季风", "温带季风", "温带季风", "亚热带海洋性", "亚热带季风", "亚热带季风", "亚热带季风", "亚热带湿润", "温带大陆性", "温带季风" };
            var pollenConcentrations = new[] { "中等", "较高", "低", "中等", "高", "较低", "中等", "较高", "低", "中等", "较高", "中等", "低", "较低", "高", "中等", "较高", "中等", "低", "较低" };

            for (int i = 1; i <= 20; i++)
            {
                var regionName = regionNames[(i - 1) % regionNames.Length];
                var climateType = climateTypes[(i - 1) % climateTypes.Length];
                var pollenConcentration = pollenConcentrations[(i - 1) % pollenConcentrations.Length];
                var greenSpaceRate = 20.0m + (i % 30); // 20-49%
                var airQualityIndex = 50 + (i % 50); // 50-99
                var avgTemperature = 8.0m + (i % 20); // 8-27度
                var humidityLevel = 40.0m + (i % 40); // 40-79%

                environments.Add(new RegionalEnvironment 
                { 
                    region_id = $"R{i:D3}", 
                    region_name = regionName, 
                    green_space_rate = greenSpaceRate, 
                    air_quality_index = airQualityIndex, 
                    pollen_concentration = pollenConcentration, 
                    climate_type = climateType, 
                    avg_temperature = avgTemperature, 
                    humidity_level = humidityLevel, 
                    update_date = DateTime.UtcNow.AddDays(-i) 
                });
            }

            _context.RegionalEnvironments.AddRange(environments);
            await _context.SaveChangesAsync();
            return $"RegionalEnvironments表插入{environments.Count}条数据";
        }

        private async Task<string> SeedHouseholdEnvironments()
        {
            if (await _context.HouseholdEnvironments.AnyAsync()) return "HouseholdEnvironments表已有数据，跳过";

            var households = new List<HouseholdEnvironment>();
            var residenceTypes = new[] { "1", "2", "3" }; // 城市、农村、其他
            var ventilationQualities = new[] { "良好", "一般", "较差", "优秀", "差" };
            var petTypes = new[] { "", "猫", "狗", "鸟", "兔子", "仓鼠", "金鱼", "乌龟", "猫狗", "其他", "", "", "", "", "", "", "", "", "", "" }; // 大部分没有宠物
            var beddingMaterials = new[] { "纯棉", "混纺", "羽绒", "丝绸", "亚麻", "竹纤维", "记忆棉", "乳胶", "聚酯纤维", "麻纤维", "天丝", "莫代尔", "法兰绒", "珊瑚绒", "毛绒", "人造毛", "合成纤维", "天然纤维", "高科技纤维", "抗菌纤维" };

            for (int i = 1; i <= 20; i++)
            {
                var residenceType = residenceTypes[(i - 1) % residenceTypes.Length];
                var buildingAge = 5 + (i % 30); // 5-34年
                var ventilationQuality = ventilationQualities[(i - 1) % ventilationQualities.Length];
                var indoorPm25 = 15.0m + (i % 40); // 15-54
                var petExposure = !string.IsNullOrEmpty(petTypes[(i - 1) % petTypes.Length]);
                var petType = petTypes[(i - 1) % petTypes.Length];
                var beddingMaterial = beddingMaterials[(i - 1) % beddingMaterials.Length];
                var recordDate = DateTime.UtcNow.AddDays(-(i + 5));

                households.Add(new HouseholdEnvironment 
                { 
                    household_id = $"HE{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    investigator_id = $"INV{((i - 1) % 20) + 1:D3}", 
                    residence_type = residenceType, 
                    building_age = buildingAge, 
                    ventilation_quality = ventilationQuality, 
                    indoor_pm25 = indoorPm25, 
                    pet_exposure = petExposure, 
                    pet_type = petType, 
                    bedding_material = beddingMaterial, 
                    record_date = recordDate 
                });
            }

            _context.HouseholdEnvironments.AddRange(households);
            await _context.SaveChangesAsync();
            return $"HouseholdEnvironments表插入{households.Count}条数据";
        }

        private async Task<string> SeedIndividualHealthBehaviors()
        {
            if (await _context.IndividualHealthBehaviors.AnyAsync()) return "IndividualHealthBehaviors表已有数据，跳过";

            var behaviors = new List<IndividualHealthBehavior>();
            var dietPatterns = new[] { "均衡饮食", "偏素食", "高蛋白", "高纤维", "低脂饮食", "地中海饮食", "素食主义", "低糖饮食", "高钙饮食", "抗炎饮食", "清淡饮食", "传统饮食", "西式饮食", "有机饮食", "无麸质饮食", "低盐饮食", "富含维生素", "偏荤食", "混合饮食", "特殊饮食" };
            var antibioticFrequencies = new[] { "偶尔", "经常", "很少", "从不", "频繁", "适中", "必要时", "医生建议", "定期", "不规律", "按需", "谨慎使用", "避免使用", "合理使用", "过度使用", "正常使用", "减少使用", "控制使用", "最小化", "遵医嘱" };
            var earlyLifeMedications = new[] { "无", "抗生素", "维生素", "益生菌", "退热药", "止咳药", "感冒药", "疫苗", "营养补充剂", "钙片", "鱼肝油", "铁剂", "锌剂", "消化药", "过敏药", "止泻药", "外用药", "中药", "保健品", "其他" };

            for (int i = 1; i <= 20; i++)
            {
                var dietPattern = dietPatterns[(i - 1) % dietPatterns.Length];
                var vitaminDLevel = 20.0m + (i % 25); // 20-44 ng/mL
                var sunExposure = (i % 3) != 0; // 大部分有日照
                var vaccinationStatus = (i % 5) != 0; // 大部分已接种
                var antibioticFrequency = antibioticFrequencies[(i - 1) % antibioticFrequencies.Length];
                var earlyLifeMedication = earlyLifeMedications[(i - 1) % earlyLifeMedications.Length];
                var smokeExposure = (i % 4) == 0; // 少部分有烟雾暴露

                behaviors.Add(new IndividualHealthBehavior 
                { 
                    individual_id = $"IHB{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    household_id = $"HE{i:D3}", 
                    investigator_id = $"INV{((i - 1) % 20) + 1:D3}", 
                    diet_pattern = dietPattern, 
                    vitamin_d_level = vitaminDLevel, 
                    sun_exposure = sunExposure, 
                    vaccination_status = vaccinationStatus, 
                    antibiotic_usage_frequency = antibioticFrequency, 
                    early_life_medication = earlyLifeMedication, 
                    smoke_exposure = smokeExposure 
                });
            }

            _context.IndividualHealthBehaviors.AddRange(behaviors);
            await _context.SaveChangesAsync();
            return $"IndividualHealthBehaviors表插入{behaviors.Count}条数据";
        }

        private async Task<string> SeedQuestionnaireData()
        {
            if (await _context.QuestionnaireDatas.AnyAsync()) return "QuestionnaireData表已有数据，跳过";

            var questionnaires = new List<QuestionnaireData>();
            var formTypes = new[] { "生活质量评估", "症状评估", "环境暴露", "家族史调查", "用药依从性", "心理健康", "营养状况", "运动习惯", "睡眠质量", "社交功能", "学习能力", "认知功能", "情绪状态", "行为评估", "发育评估", "预后评估", "康复评估", "满意度调查", "风险评估", "健康教育" };
            var dataSources = new[] { "儿童哮喘生活质量问卷", "过敏性鼻炎症状评分", "环境因素暴露问卷", "家族过敏史问卷", "Morisky用药依从性量表", "儿童抑郁量表", "营养风险筛查工具", "体力活动问卷", "匹兹堡睡眠质量指数", "社会支持评定量表", "学习困难筛查表", "认知能力评估量表", "情绪行为检查表", "Conners行为量表", "Denver发育筛查量表", "预后评估量表", "功能独立性评定量表", "患者满意度量表", "健康风险评估问卷", "健康知识调查表" };
            var rawDataTemplates = new[] 
            { 
                "{'q1':3,'q2':4,'q3':2,'total_score':45}",
                "{'nasal_congestion':3,'rhinorrhea':2,'sneezing':4,'total_score':32}",
                "{'home_environment':2,'school_environment':3,'outdoor_activities':4,'total_score':38}",
                "{'family_history':1,'genetic_risk':2,'environmental_factors':3,'total_score':28}",
                "{'medication_adherence':4,'missed_doses':1,'total_score':42}",
                "{'depression_score':8,'anxiety_score':6,'total_score':35}",
                "{'nutrition_risk':2,'dietary_intake':3,'total_score':25}",
                "{'exercise_frequency':4,'exercise_intensity':3,'total_score':40}",
                "{'sleep_quality':2,'sleep_duration':3,'total_score':18}",
                "{'social_support':4,'family_support':5,'total_score':48}",
                "{'learning_difficulty':1,'academic_performance':3,'total_score':22}",
                "{'memory':4,'attention':3,'processing_speed':3,'total_score':50}",
                "{'emotional_stability':3,'mood_regulation':2,'total_score':30}",
                "{'hyperactivity':2,'attention_deficit':1,'total_score':15}",
                "{'motor_skills':4,'language_skills':4,'total_score':55}",
                "{'disease_progression':2,'treatment_response':4,'total_score':35}",
                "{'functional_independence':3,'rehabilitation_progress':4,'total_score':42}",
                "{'service_satisfaction':5,'care_quality':4,'total_score':48}",
                "{'health_risks':2,'lifestyle_factors':3,'total_score':28}",
                "{'health_knowledge':4,'health_behaviors':3,'total_score':38}"
            };

            for (int i = 1; i <= 20; i++)
            {
                var formType = formTypes[(i - 1) % formTypes.Length];
                var dataSource = dataSources[(i - 1) % dataSources.Length];
                var rawData = rawDataTemplates[(i - 1) % rawDataTemplates.Length];
                var fillDate = DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd");

                questionnaires.Add(new QuestionnaireData 
                { 
                    questionnaire_id = $"Q{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    form_type = formType, 
                    fill_date = fillDate, 
                    data_source = dataSource, 
                    investigator_id = $"INV{((i - 1) % 20) + 1:D3}", 
                    raw_data = rawData, 
                    create_time = DateTime.UtcNow 
                });
            }

            _context.QuestionnaireDatas.AddRange(questionnaires);
            await _context.SaveChangesAsync();
            return $"QuestionnaireData表插入{questionnaires.Count}条数据";
        }

        private async Task<string> SeedSpecimenInfos()
        {
            if (await _context.SpecimenInfos.AnyAsync()) return "SpecimenInfos表已有数据，跳过";

            var specimens = new List<SpecimenInfo>();
            var specimenTypes = new[] { "血液", "唾液", "尿液", "鼻咽拭子", "粪便", "组织", "脑脊液", "胸腔积液", "腹腔积液", "关节液", "痰液", "鼻分泌物", "眼分泌物", "耳分泌物", "伤口分泌物", "皮肤组织", "毛发", "指甲", "呼出气", "汗液" };
            var collectionSites = new[] { "北京儿童医院", "上海儿科研究所", "广州医科大学", "深圳儿童医院", "天津医科大学", "南京儿童医院", "武汉协和医院", "成都华西医院", "西安儿童医院", "重庆医科大学", "杭州儿童医院", "青岛妇女儿童医院", "大连儿童医院", "厦门大学附属医院", "苏州儿童医院", "宁波妇女儿童医院", "无锡儿童医院", "长沙湘雅医院", "郑州儿童医院", "济南儿童医院" };
            var storageConditions = new[] { "-80°C", "-20°C", "4°C", "室温", "-196°C", "冷冻干燥", "甲醛固定", "石蜡包埋", "液氮保存", "冷藏保存", "超低温保存", "常温保存", "真空保存", "充氮保存", "防腐保存", "冷冻保存", "干燥保存", "密封保存", "避光保存", "无菌保存" };
            var storageLocations = new[] { "生物样本库A区", "生物样本库B区", "生物样本库C区", "生物样本库D区", "生物样本库E区", "中央样本库", "临床样本库", "研究样本库", "基因样本库", "蛋白样本库", "代谢样本库", "免疫样本库", "病理样本库", "微生物样本库", "环境样本库", "质控样本库", "备份样本库", "长期保存库", "临时保存库", "特殊样本库" };

            for (int i = 1; i <= 20; i++)
            {
                var specimenType = specimenTypes[(i - 1) % specimenTypes.Length];
                var collectionSite = collectionSites[(i - 1) % collectionSites.Length];
                var storageCondition = storageConditions[(i - 1) % storageConditions.Length];
                var storageLocation = storageLocations[(i - 1) % storageLocations.Length];
                var collectionDate = DateTime.UtcNow.AddDays(-(i + 5));
                var volumeMl = (specimenType == "鼻咽拭子" || specimenType == "组织") ? null : (decimal?)(1.0m + (i % 15)); // 1-15ml

                specimens.Add(new SpecimenInfo 
                { 
                    specimen_id = $"SP{i:D3}", 
                    patient_id = $"P{i:D3}", 
                    collection_date = collectionDate, 
                    specimen_type = specimenType, 
                    collection_site = collectionSite, 
                    volume_ml = volumeMl, 
                    storage_condition = storageCondition, 
                    storage_location = storageLocation 
                });
            }

            _context.SpecimenInfos.AddRange(specimens);
            await _context.SaveChangesAsync();
            return $"SpecimenInfos表插入{specimens.Count}条数据";
        }

        private async Task<string> SeedSpecimenQualities()
        {
            if (await _context.SpecimenQualities.AnyAsync()) return "SpecimenQualities表已有数据，跳过";

            var qualities = new List<SpecimenQuality>();
            var qualityStatuses = new[] { "优良", "良好", "一般", "较差", "优秀", "合格", "不合格", "可用", "不可用", "高质量", "中等质量", "低质量", "标准", "超标", "合规", "异常", "正常", "可接受", "不可接受", "待检" };

            for (int i = 1; i <= 20; i++)
            {
                var dnaConcentration = 50.0m + (i % 150); // 50-199 ng/μL
                var rnaQuality = 5.0m + (i % 5) * 0.5m; // 5.0-7.5 RIN值
                var proteinConcentration = 1.0m + (i % 4) * 0.5m; // 1.0-3.0 mg/mL
                var qualityStatus = qualityStatuses[(i - 1) % qualityStatuses.Length];

                qualities.Add(new SpecimenQuality 
                { 
                    specimen_id = $"SP{i:D3}", 
                    dna_concentration = dnaConcentration, 
                    rna_quality = rnaQuality, 
                    protein_concentration = proteinConcentration, 
                    quality_status = qualityStatus 
                });
            }

            _context.SpecimenQualities.AddRange(qualities);
            await _context.SaveChangesAsync();
            return $"SpecimenQualities表插入{qualities.Count}条数据";
        }

        private async Task<string> SeedGenomicData()
        {
            if (await _context.GenomicDatas.AnyAsync()) return "GenomicData表已有数据，跳过";

            var genomicData = new List<GenomicData>();
            var il4Genotypes = new[] { "CC", "CT", "TT", "CG", "GG", "TG", "AC", "AG", "AT", "GT", "AA", "TC", "GC", "TA", "CA", "GA", "CX", "TX", "GX", "AX" };
            var il13Genotypes = new[] { "GG", "GA", "AA", "GT", "AT", "TT", "GC", "AC", "CC", "TC", "AG", "CG", "GX", "AX", "TX", "CX", "XG", "XA", "XT", "XC" };

            for (int i = 1; i <= 20; i++)
            {
                var il4Genotype = il4Genotypes[(i - 1) % il4Genotypes.Length];
                var il13Genotype = il13Genotypes[(i - 1) % il13Genotypes.Length];
                var analysisDate = DateTime.UtcNow.AddDays(-(i + 3));
                var dataPath = $"/genomic_data/patient_P{i:D3}/results.vcf";

                genomicData.Add(new GenomicData 
                { 
                    specimen_id = $"SP{i:D3}", 
                    il4_genotype = il4Genotype, 
                    il13_genotype = il13Genotype, 
                    analysis_date = analysisDate, 
                    data_path = dataPath 
                });
            }

            _context.GenomicDatas.AddRange(genomicData);
            await _context.SaveChangesAsync();
            return $"GenomicData表插入{genomicData.Count}条数据";
        }

        private async Task<string> SeedProteinData()
        {
            if (await _context.ProteinDatas.AnyAsync()) return "ProteinData表已有数据，跳过";

            var proteinData = new List<ProteinData>();

            for (int i = 1; i <= 20; i++)
            {
                var igeLevel = 250.0m + (i % 300); // 250-549 IU/mL
                var il4Level = 10.0m + (i % 20) * 0.5m; // 10.0-19.5 pg/ml
                var il13Level = 6.0m + (i % 15) * 0.5m; // 6.0-13.5 pg/ml
                var il5Level = 8.0m + (i % 12) * 0.5m; // 8.0-13.5 pg/ml
                var cytokineProfile = $"IL-4: {il4Level:F1} pg/ml, IL-13: {il13Level:F1} pg/ml, IL-5: {il5Level:F1} pg/ml";
                var analysisDate = DateTime.UtcNow.AddDays(-(i + 3));

                proteinData.Add(new ProteinData 
                { 
                    specimen_id = $"SP{i:D3}", 
                    ige_level = igeLevel, 
                    cytokine_profile = cytokineProfile, 
                    analysis_date = analysisDate 
                });
            }

            _context.ProteinDatas.AddRange(proteinData);
            await _context.SaveChangesAsync();
            return $"ProteinData表插入{proteinData.Count}条数据";
        }

        private async Task<string> SeedClinicalData()
        {
            if (await _context.ClinicalDatas.AnyAsync()) return "ClinicalData表已有数据，跳过";

            var clinicalData = new List<ClinicalData>();
            var diseaseStages = new[] { "轻度", "中度", "重度", "极重度", "缓解期", "急性期", "稳定期", "恶化期", "初期", "进展期", "终末期", "康复期", "复发期", "慢性期", "亚急性期", "临界期", "活动期", "静止期", "代偿期", "失代偿期" };

            for (int i = 1; i <= 20; i++)
            {
                var diseaseStage = diseaseStages[(i - 1) % diseaseStages.Length];
                var symptomScore = 10 + (i % 40); // 10-49分

                clinicalData.Add(new ClinicalData 
                { 
                    specimen_id = $"SP{i:D3}", 
                    disease_stage = diseaseStage, 
                    symptom_score = symptomScore 
                });
            }

            _context.ClinicalDatas.AddRange(clinicalData);
            await _context.SaveChangesAsync();
            return $"ClinicalData表插入{clinicalData.Count}条数据";
        }

        private async Task<string> SeedMemos()
        {
            if (await _context.Memos.AnyAsync()) return "Memos表已有数据，跳过";

            // 动态获取所有用户ID
            var allUsers = await _context.Users.ToListAsync();
            if (allUsers.Count == 0)
            {
                return "Memos表插入失败：未找到用户记录";
            }

            var memos = new List<Memo>();
            var titles = new[] { "患者随访提醒", "研究数据分析", "药物不良反应记录", "样本收集计划", "实验结果整理", "病例讨论准备", "学术会议安排", "质量控制检查", "设备维护提醒", "培训资料准备", "临床试验跟进", "统计分析任务", "文献整理工作", "报告撰写计划", "数据备份检查", "安全培训安排", "预算审核工作", "设备采购申请", "学术论文修改", "项目进度汇报" };
            var contents = new[] 
            { 
                "下周二需要复查肺功能",
                "完成本月过敏患者数据统计分析",
                "患者出现轻微皮疹，需要调整用药方案",
                "本周需要收集5例新患者的生物样本",
                "整理上月实验数据，准备季度报告",
                "周三下午病例讨论会，准备疑难病例",
                "参加国际过敏学会年会，准备发言稿",
                "检查实验室质量控制流程，确保数据准确性",
                "实验设备需要定期维护，联系技术人员",
                "准备新员工培训材料，安排培训时间",
                "跟进临床试验患者情况，更新试验记录",
                "使用SPSS分析患者基线数据",
                "整理最新文献资料，更新参考文献库",
                "撰写月度工作总结报告",
                "检查数据库备份情况，确保数据安全",
                "组织实验室安全培训，提高安全意识",
                "审核部门预算执行情况",
                "申请新的实验设备采购",
                "修改学术论文，准备投稿",
                "准备项目进度汇报材料"
            };

            for (int i = 1; i <= 20; i++)
            {
                var user = allUsers[(i - 1) % allUsers.Count]; // 轮换使用所有用户
                var title = titles[(i - 1) % titles.Length];
                var content = contents[(i - 1) % contents.Length];
                var isDone = (i % 3) == 0 ? 1 : 0; // 1/3的任务已完成

                memos.Add(new Memo 
                { 
                    userid = user.Id, 
                    title = title, 
                    content = content, 
                    isdone = isDone 
                });
            }

            _context.Memos.AddRange(memos);
            await _context.SaveChangesAsync();
            return $"Memos表插入{memos.Count}条数据";
        }

        [HttpPost("regenerate-patient-data")]
        public async Task<IActionResult> RegeneratePatientData()
        {
            try
            {
                var results = new List<string>();

                // 1. 清空所有患者相关数据
                _context.FollowUpRecords.RemoveRange(_context.FollowUpRecords);
                _context.MedicationRecords.RemoveRange(_context.MedicationRecords);
                _context.PhysicalExaminations.RemoveRange(_context.PhysicalExaminations);
                _context.Diagnoses.RemoveRange(_context.Diagnoses);
                _context.MedicalCosts.RemoveRange(_context.MedicalCosts);
                _context.LabTests.RemoveRange(_context.LabTests);
                _context.ImagingDetails.RemoveRange(_context.ImagingDetails);
                _context.PulmonaryDetails.RemoveRange(_context.PulmonaryDetails);
                _context.RegionalEnvironments.RemoveRange(_context.RegionalEnvironments);
                _context.HouseholdEnvironments.RemoveRange(_context.HouseholdEnvironments);
                _context.IndividualHealthBehaviors.RemoveRange(_context.IndividualHealthBehaviors);
                _context.QuestionnaireDatas.RemoveRange(_context.QuestionnaireDatas);
                _context.SpecimenQualities.RemoveRange(_context.SpecimenQualities);
                _context.GenomicDatas.RemoveRange(_context.GenomicDatas);
                _context.ProteinDatas.RemoveRange(_context.ProteinDatas);
                _context.ClinicalDatas.RemoveRange(_context.ClinicalDatas);
                _context.SpecimenInfos.RemoveRange(_context.SpecimenInfos);
                _context.PatientStaffRelations.RemoveRange(_context.PatientStaffRelations);
                _context.FamilyHistories.RemoveRange(_context.FamilyHistories);
                _context.MedicalHistories.RemoveRange(_context.MedicalHistories);
                _context.Contacts.RemoveRange(_context.Contacts);
                _context.Insurance.RemoveRange(_context.Insurance);
                _context.PatientBasicInfos.RemoveRange(_context.PatientBasicInfos);
                // Patients表已删除，无需清理
                await _context.SaveChangesAsync();

                // 2. 确保有基础用户和管理员数据
                if (!await _context.Users.AnyAsync())
                {
                    results.Add(await SeedUsers());
                    results.Add(await SeedAdmins());
                }

                // 确保有调研员数据
                if (!await _context.InvestigatorQualifications.AnyAsync())
                {
                    results.Add(await SeedInvestigatorQualifications());
                }

                // 3. 重新生成完整的患者数据
                var coreResults = new List<string>();
                // SeedPatients已删除，只使用PatientBasicInfo表
                coreResults.Add(await SeedPatientBasicInfos()); // 患者详细信息表
                coreResults.Add(await SeedInsurance());
                coreResults.Add(await SeedContacts());
                coreResults.Add(await SeedMedicalHistory());
                coreResults.Add(await SeedFamilyHistory());
                coreResults.Add(await SeedPatientStaffRelations());

                // 临床数据
                var clinicalResults = new List<string>();
                clinicalResults.Add(await SeedDiagnoses());
                clinicalResults.Add(await SeedPhysicalExaminations());
                clinicalResults.Add(await SeedMedicationRecords());
                clinicalResults.Add(await SeedFollowUpRecords());
                clinicalResults.Add(await SeedMedicalCosts());

                // 检查数据
                var examResults = new List<string>();
                examResults.Add(await SeedLabTests());
                examResults.Add(await SeedImagingDetails());
                examResults.Add(await SeedPulmonaryDetails());

                // 调研数据
                var researchResults = new List<string>();
                researchResults.Add(await SeedRegionalEnvironments());
                researchResults.Add(await SeedHouseholdEnvironments());
                researchResults.Add(await SeedIndividualHealthBehaviors());
                researchResults.Add(await SeedQuestionnaireData());

                // 实验数据
                var labResults = new List<string>();
                labResults.Add(await SeedSpecimenInfos());
                labResults.Add(await SeedSpecimenQualities());
                labResults.Add(await SeedGenomicData());
                labResults.Add(await SeedProteinData());
                labResults.Add(await SeedClinicalData());

                // 其他数据
                var otherResults = new List<string>();
                otherResults.Add(await SeedMemos());

                results.AddRange(coreResults);
                results.AddRange(clinicalResults);
                results.AddRange(examResults);
                results.AddRange(researchResults);
                results.AddRange(labResults);
                results.AddRange(otherResults);

                // 验证数据生成结果
                // Patients表已删除，只统计PatientBasicInfo
                var basicInfoCount = await _context.PatientBasicInfos.CountAsync();
                var followupCount = await _context.FollowUpRecords.CountAsync();
                var specimenCount = await _context.SpecimenInfos.CountAsync();

                return Ok(new { 
                    success = true, 
                    message = $"✅ 患者数据重新生成完成！",
                    details = new {
                        patientRecords = basicInfoCount,
                        basicInfoRecords = basicInfoCount,
                        followupRecords = followupCount,
                        specimenRecords = specimenCount,
                        dataCategories = 9,
                        note = "患者列表和患者详情现在使用统一的数据源"
                    },
                    summary = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "患者数据重新生成失败: " + ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }
    }
} 