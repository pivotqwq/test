using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static backend.Data.ApplicationDbContext;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public class Memo
        {
            public int id { get; set; }
            public string userid { get; set; }
            public string title { get; set; }
            public string content { get; set; }
            public DateTime created_at { get; set; } = DateTime.UtcNow;
            public int isdone { get; set; }
        }
        public DbSet<Memo> Memos { get; set; }
        // 医保表
        public DbSet<Insurance> Insurance { get; set; }

        // 联系人表
        public DbSet<Contact> Contacts { get; set; }

        // 既往病史表
        public DbSet<MedicalHistory> MedicalHistories { get; set; }

        // 家族病史表
        public DbSet<FamilyHistory> FamilyHistories { get; set; }

        // 实验室检查表
        public DbSet<LabTest> LabTests { get; set; }

        // 影像学检查详情表
        public DbSet<ImagingDetail> ImagingDetails { get; set; }

        // 肺功能检查详情表
        public DbSet<PulmonaryDetail> PulmonaryDetails { get; set; }

        // 疾病诊断表
        public DbSet<Diagnosis> Diagnoses { get; set; }
        // 患儿基本信息表
        public DbSet<PatientBasicInfo> PatientBasicInfos { get; set; }

        // 数据采集者资质信息表
        public DbSet<InvestigatorQualification> InvestigatorQualifications { get; set; }

        // 家庭环境监测数据表
        public DbSet<HouseholdEnvironment> HouseholdEnvironments { get; set; }

        // 个人健康行为数据表
        public DbSet<IndividualHealthBehavior> IndividualHealthBehaviors { get; set; }

        // 区域环境数据表
        public DbSet<RegionalEnvironment> RegionalEnvironments { get; set; }

        // 问卷调查数据表
        public DbSet<QuestionnaireData> QuestionnaireDatas { get; set; }
        public DbSet<PatientStaffRelation> PatientStaffRelations { get; set; }
        public DbSet<PhysicalExamination> PhysicalExaminations { get; set; }
        public DbSet<MedicationRecord> MedicationRecords { get; set; }
        public DbSet<FollowUpRecord> FollowUpRecords { get; set; }
        public DbSet<MedicalCost> MedicalCosts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SpecimenInfo> SpecimenInfos { get; set; }
        public DbSet<SpecimenQuality> SpecimenQualities { get; set; }
        public DbSet<GenomicData> GenomicDatas { get; set; }
        public DbSet<ProteinData> ProteinDatas { get; set; }
        public DbSet<ClinicalData> ClinicalDatas { get; set; }
    }

    public class User
    {
        [Key]
        [MaxLength(36)]  // UUID 字符串长度为36
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? username { get; set; }
        public string? password { get; set; }
        public string? email { get; set; }
        public string? urlBase64 { get; set; }
        public string? phone { get; set; }
        public string? name {get; set; }
        public string? profession { get; set; }
    }

    public class Insurance
    {
        [Key]
        public string insurance_id { get; set; }
        public string patient_id { get; set; }
        public string insurance_type { get; set; }
    }

    public class Contact
    {
        [Key]
        public string contact_id { get; set; }
        public string patient_id { get; set; }
        public string name { get; set; }
        public string contact_info { get; set; }
    }

    public class MedicalHistory
    {
        [Key]
        public string history_id { get; set; }
        public string patient_id { get; set; }
        public string allergy_history { get; set; }
    }

    public class FamilyHistory
    {
        [Key]
        public string family_history_id { get; set; }
        public string patient_id { get; set; }
        public string allergy_history { get; set; }
    }

    public class LabTest
    {
        [Key]
        public string lab_id { get; set; }
        public string patient_id { get; set; }
        public string item_name { get; set; }
        public string exam_value { get; set; }
        public string exam_type { get; set; }
    }

    public class ImagingDetail
    {
        [Key]
        public string imaging_id { get; set; }
        public string lab_id { get; set; }
        public string exam_details { get; set; }
    }

    public class PulmonaryDetail
    {
        [Key]
        public string pulmonary_id { get; set; }
        public string lab_id { get; set; }
        public string exam_details { get; set; }
    }

    public class Diagnosis
    {
        [Key]
        public string diagnosis_id { get; set; }
        public string patient_id { get; set; }
        public string disease_name { get; set; }
        public string severity { get; set; }
        public string description { get; set; }
    }
    public class PatientBasicInfo
    {
        [Key]
        [MaxLength(20)]
        public string? patient_id { get; set; }

        [MaxLength(50)]
        public string? name { get; set; }

        [MaxLength(1)]
        public string? gender { get; set; }

        public DateTime birth_date { get; set; }

        public decimal? age_at_diagnosi { get; set; }

        [MaxLength(1)]
        public string? residence_type { get; set; }

        public string? allergy_history { get; set; }

        [MaxLength(20)]
        public string? phone { get; set; }

        public DateTime create_time { get; set; } = DateTime.UtcNow;

        public DateTime? update_time { get; set; }
    }
    public class InvestigatorQualification
    {
        [Key]
        [MaxLength(20)]
        public string investigator_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [MaxLength(100)]
        public string qualification { get; set; }

        [Required]
        [MaxLength(100)]
        public string institution { get; set; }

        [MaxLength(50)]
        public string position { get; set; }

        [MaxLength(20)]
        public string contact_phone { get; set; }

        public DateTime create_time { get; set; } = DateTime.UtcNow;
    }
    public class HouseholdEnvironment
    {
        [Key]
        [MaxLength(20)]
        public string household_id { get; set; }

        [MaxLength(20)]
        public string patient_id { get; set; }

        [Required]
        public string residence_type { get; set; }

        public int? building_age { get; set; }

        public string ventilation_quality { get; set; }

        public decimal? indoor_pm25 { get; set; }

        public bool pet_exposure { get; set; }

        [MaxLength(50)]
        public string pet_type { get; set; }

        public string bedding_material { get; set; }

        public DateTime record_date { get; set; }

        [MaxLength(20)]
        public string investigator_id { get; set; }

        [ForeignKey("patient_id")]
        public PatientBasicInfo patient_basic_info { get; set; }

        [ForeignKey("investigator_id")]
        public InvestigatorQualification investigator_qualification { get; set; }
    }
    public class IndividualHealthBehavior
    {
        [Key]
        [MaxLength(20)]
        public string individual_id { get; set; }

        [MaxLength(20)]
        public string patient_id { get; set; }

        [MaxLength(20)]
        public string household_id { get; set; }

        public string diet_pattern { get; set; }

        public decimal? vitamin_d_level { get; set; }

        public bool sun_exposure { get; set; }

        public bool vaccination_status { get; set; }

        public string antibiotic_usage_frequency { get; set; }

        public string early_life_medication { get; set; }

        public bool smoke_exposure { get; set; }

        [MaxLength(20)]
        public string investigator_id { get; set; }

        [ForeignKey("patient_id")]
        public PatientBasicInfo patient_basic_info { get; set; }

        [ForeignKey("investigator_id")]
        public InvestigatorQualification investigator_qualification { get; set; }
    }
    public class RegionalEnvironment
    {
        [Key]
        [MaxLength(20)]
        public string region_id { get; set; }

        [MaxLength(100)]
        public string region_name { get; set; }

        public decimal? green_space_rate { get; set; }

        public int? air_quality_index { get; set; }

        public string pollen_concentration { get; set; }

        public string climate_type { get; set; }

        public decimal? avg_temperature { get; set; }

        public decimal? humidity_level { get; set; }

        public DateTime update_date { get; set; }
    }
    public class QuestionnaireData
    {
        [Key]
        [MaxLength(20)]
        public string questionnaire_id { get; set; }

        [MaxLength(20)]
        public string patient_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string form_type { get; set; }

        [Required]
        public string fill_date { get; set; }

        [MaxLength(50)]
        public string data_source { get; set; }

        [MaxLength(20)]
        public string investigator_id { get; set; }

        public string raw_data { get; set; }

        public DateTime create_time { get; set; } = DateTime.UtcNow;

        [ForeignKey("patient_id")]
        public PatientBasicInfo patient_basic_info { get; set; }

        [ForeignKey("investigator_id")]
        public InvestigatorQualification investigator_qualification { get; set; }
    }
    public class PatientStaffRelation
    {
        [Key]
        public int relation_id { get; set; }
        public string patient_id { get; set; }
        public string staff_id { get; set; }
        public string relation_type { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
    public class PhysicalExamination
    {
        [Key]
        public int exam_id { get; set; }
        public string patient_id { get; set; }
        public DateTime exam_date { get; set; }
        public decimal? temperature { get; set; }
        public int? pulse { get; set; }
        public int? oxygen_saturation { get; set; }
        public string lung_sounds { get; set; }
        public string rash_description { get; set; }
    }
    public class MedicationRecord
    {
        [Key]
        public int medication_id { get; set; }
        public string patient_id { get; set; }
        public string drug_name { get; set; }
        public string dosage { get; set; }
        public string frequency { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string drug_category { get; set; }
    }
    public class FollowUpRecord
    {
        [Key]
        public int followup_id { get; set; }
        public string patient_id { get; set; }
        public DateTime followup_date { get; set; }
        public string symptom_improvement { get; set; }
        public string adverse_effects { get; set; }
        public int? act_score { get; set; }
    }
    public class MedicalCost
    {
        [Key]
        public int cost_id { get; set; }
        public string patient_id { get; set; }
        public string cost_type { get; set; }
        public decimal amount { get; set; }
        public DateTime cost_date { get; set; }
    }
    public class Admin
    {
        [Key]
        public string user_id { get; set; }
        public bool is_admin { get; set; }
    }
    public class SpecimenInfo
    {
        [Key]
        public string specimen_id { get; set; }
        public string patient_id { get; set; }
        public DateTime collection_date { get; set; }
        public string specimen_type { get; set; }
        public string collection_site { get; set; }
        public decimal? volume_ml { get; set; }
        public string storage_condition { get; set; }
        public string storage_location { get; set; }
    }
    public class SpecimenQuality
    {
        [Key]
        public int quality_id { get; set; }
        public string specimen_id { get; set; }
        public decimal? dna_concentration { get; set; }
        public decimal? rna_quality { get; set; }
        public decimal? protein_concentration { get; set; }
        public string quality_status { get; set; }
    }
    public class GenomicData
    {
        [Key]
        public int data_id { get; set; }
        public string specimen_id { get; set; }
        public string il4_genotype { get; set; }
        public string il13_genotype { get; set; }
        public DateTime? analysis_date { get; set; }
        public string data_path { get; set; }
    }
    public class ProteinData
    {
        [Key]
        public int data_id { get; set; }
        public string specimen_id { get; set; }
        public decimal? ige_level { get; set; }
        public string cytokine_profile { get; set; }
        public DateTime? analysis_date { get; set; }
    }
    public class ClinicalData
    {
        [Key]
        public int association_id { get; set; }
        public string specimen_id { get; set; }
        public string disease_stage { get; set; }
        public int? symptom_score { get; set; }
    }
}
