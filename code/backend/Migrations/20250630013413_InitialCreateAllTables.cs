using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalDatas",
                columns: table => new
                {
                    association_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    specimen_id = table.Column<string>(type: "text", nullable: false),
                    disease_stage = table.Column<string>(type: "text", nullable: false),
                    symptom_score = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalDatas", x => x.association_id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    contact_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    contact_info = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.contact_id);
                });

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    diagnosis_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    disease_name = table.Column<string>(type: "text", nullable: false),
                    severity = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.diagnosis_id);
                });

            migrationBuilder.CreateTable(
                name: "FamilyHistories",
                columns: table => new
                {
                    family_history_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    allergy_history = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyHistories", x => x.family_history_id);
                });

            migrationBuilder.CreateTable(
                name: "FollowUpRecords",
                columns: table => new
                {
                    followup_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    followup_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    symptom_improvement = table.Column<string>(type: "text", nullable: false),
                    adverse_effects = table.Column<string>(type: "text", nullable: false),
                    act_score = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpRecords", x => x.followup_id);
                });

            migrationBuilder.CreateTable(
                name: "GenomicDatas",
                columns: table => new
                {
                    data_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    specimen_id = table.Column<string>(type: "text", nullable: false),
                    il4_genotype = table.Column<string>(type: "text", nullable: false),
                    il13_genotype = table.Column<string>(type: "text", nullable: false),
                    analysis_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenomicDatas", x => x.data_id);
                });

            migrationBuilder.CreateTable(
                name: "ImagingDetails",
                columns: table => new
                {
                    imaging_id = table.Column<string>(type: "text", nullable: false),
                    lab_id = table.Column<string>(type: "text", nullable: false),
                    exam_details = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagingDetails", x => x.imaging_id);
                });

            migrationBuilder.CreateTable(
                name: "Insurance",
                columns: table => new
                {
                    insurance_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    insurance_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurance", x => x.insurance_id);
                });

            migrationBuilder.CreateTable(
                name: "InvestigatorQualifications",
                columns: table => new
                {
                    investigator_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    qualification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    institution = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestigatorQualifications", x => x.investigator_id);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    lab_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    item_name = table.Column<string>(type: "text", nullable: false),
                    exam_value = table.Column<string>(type: "text", nullable: false),
                    exam_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.lab_id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalCosts",
                columns: table => new
                {
                    cost_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    cost_type = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    cost_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCosts", x => x.cost_id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistories",
                columns: table => new
                {
                    history_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    allergy_history = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistories", x => x.history_id);
                });

            migrationBuilder.CreateTable(
                name: "MedicationRecords",
                columns: table => new
                {
                    medication_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    drug_name = table.Column<string>(type: "text", nullable: false),
                    dosage = table.Column<string>(type: "text", nullable: false),
                    frequency = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    drug_category = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationRecords", x => x.medication_id);
                });

            migrationBuilder.CreateTable(
                name: "Memos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isdone = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PatientBasicInfos",
                columns: table => new
                {
                    patient_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    gender = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    age_at_diagnosi = table.Column<decimal>(type: "numeric", nullable: true),
                    residence_type = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    allergy_history = table.Column<string>(type: "text", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientBasicInfos", x => x.patient_id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    medical_record_no = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: true),
                    birth_date = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PatientStaffRelations",
                columns: table => new
                {
                    relation_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    staff_id = table.Column<string>(type: "text", nullable: false),
                    relation_type = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientStaffRelations", x => x.relation_id);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalExaminations",
                columns: table => new
                {
                    exam_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    exam_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    temperature = table.Column<decimal>(type: "numeric", nullable: true),
                    pulse = table.Column<int>(type: "integer", nullable: true),
                    oxygen_saturation = table.Column<int>(type: "integer", nullable: true),
                    lung_sounds = table.Column<string>(type: "text", nullable: false),
                    rash_description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalExaminations", x => x.exam_id);
                });

            migrationBuilder.CreateTable(
                name: "ProteinDatas",
                columns: table => new
                {
                    data_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    specimen_id = table.Column<string>(type: "text", nullable: false),
                    ige_level = table.Column<decimal>(type: "numeric", nullable: true),
                    cytokine_profile = table.Column<string>(type: "text", nullable: false),
                    analysis_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProteinDatas", x => x.data_id);
                });

            migrationBuilder.CreateTable(
                name: "PulmonaryDetails",
                columns: table => new
                {
                    pulmonary_id = table.Column<string>(type: "text", nullable: false),
                    lab_id = table.Column<string>(type: "text", nullable: false),
                    exam_details = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PulmonaryDetails", x => x.pulmonary_id);
                });

            migrationBuilder.CreateTable(
                name: "RegionalEnvironments",
                columns: table => new
                {
                    region_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    region_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    green_space_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    air_quality_index = table.Column<int>(type: "integer", nullable: true),
                    pollen_concentration = table.Column<string>(type: "text", nullable: false),
                    climate_type = table.Column<string>(type: "text", nullable: false),
                    avg_temperature = table.Column<decimal>(type: "numeric", nullable: true),
                    humidity_level = table.Column<decimal>(type: "numeric", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalEnvironments", x => x.region_id);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenInfos",
                columns: table => new
                {
                    specimen_id = table.Column<string>(type: "text", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    collection_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    specimen_type = table.Column<string>(type: "text", nullable: false),
                    collection_site = table.Column<string>(type: "text", nullable: false),
                    volume_ml = table.Column<decimal>(type: "numeric", nullable: true),
                    storage_condition = table.Column<string>(type: "text", nullable: false),
                    storage_location = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenInfos", x => x.specimen_id);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenQualities",
                columns: table => new
                {
                    quality_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    specimen_id = table.Column<string>(type: "text", nullable: false),
                    dna_concentration = table.Column<decimal>(type: "numeric", nullable: true),
                    rna_quality = table.Column<decimal>(type: "numeric", nullable: true),
                    protein_concentration = table.Column<decimal>(type: "numeric", nullable: true),
                    quality_status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenQualities", x => x.quality_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    username = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    urlBase64 = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    profession = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdEnvironments",
                columns: table => new
                {
                    household_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    patient_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    residence_type = table.Column<string>(type: "text", nullable: false),
                    building_age = table.Column<int>(type: "integer", nullable: true),
                    ventilation_quality = table.Column<string>(type: "text", nullable: false),
                    indoor_pm25 = table.Column<decimal>(type: "numeric", nullable: true),
                    pet_exposure = table.Column<bool>(type: "boolean", nullable: false),
                    pet_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bedding_material = table.Column<string>(type: "text", nullable: false),
                    record_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    investigator_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdEnvironments", x => x.household_id);
                    table.ForeignKey(
                        name: "FK_HouseholdEnvironments_InvestigatorQualifications_investigat~",
                        column: x => x.investigator_id,
                        principalTable: "InvestigatorQualifications",
                        principalColumn: "investigator_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HouseholdEnvironments_PatientBasicInfos_patient_id",
                        column: x => x.patient_id,
                        principalTable: "PatientBasicInfos",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndividualHealthBehaviors",
                columns: table => new
                {
                    individual_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    patient_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    household_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    diet_pattern = table.Column<string>(type: "text", nullable: false),
                    vitamin_d_level = table.Column<decimal>(type: "numeric", nullable: true),
                    sun_exposure = table.Column<bool>(type: "boolean", nullable: false),
                    vaccination_status = table.Column<bool>(type: "boolean", nullable: false),
                    antibiotic_usage_frequency = table.Column<string>(type: "text", nullable: false),
                    early_life_medication = table.Column<string>(type: "text", nullable: false),
                    smoke_exposure = table.Column<bool>(type: "boolean", nullable: false),
                    investigator_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualHealthBehaviors", x => x.individual_id);
                    table.ForeignKey(
                        name: "FK_IndividualHealthBehaviors_InvestigatorQualifications_invest~",
                        column: x => x.investigator_id,
                        principalTable: "InvestigatorQualifications",
                        principalColumn: "investigator_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndividualHealthBehaviors_PatientBasicInfos_patient_id",
                        column: x => x.patient_id,
                        principalTable: "PatientBasicInfos",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireDatas",
                columns: table => new
                {
                    questionnaire_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    patient_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    form_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fill_date = table.Column<string>(type: "text", nullable: false),
                    data_source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    investigator_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    raw_data = table.Column<string>(type: "text", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireDatas", x => x.questionnaire_id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireDatas_InvestigatorQualifications_investigator_~",
                        column: x => x.investigator_id,
                        principalTable: "InvestigatorQualifications",
                        principalColumn: "investigator_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionnaireDatas_PatientBasicInfos_patient_id",
                        column: x => x.patient_id,
                        principalTable: "PatientBasicInfos",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdEnvironments_investigator_id",
                table: "HouseholdEnvironments",
                column: "investigator_id");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdEnvironments_patient_id",
                table: "HouseholdEnvironments",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualHealthBehaviors_investigator_id",
                table: "IndividualHealthBehaviors",
                column: "investigator_id");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualHealthBehaviors_patient_id",
                table: "IndividualHealthBehaviors",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireDatas_investigator_id",
                table: "QuestionnaireDatas",
                column: "investigator_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireDatas_patient_id",
                table: "QuestionnaireDatas",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "ClinicalDatas");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "FamilyHistories");

            migrationBuilder.DropTable(
                name: "FollowUpRecords");

            migrationBuilder.DropTable(
                name: "GenomicDatas");

            migrationBuilder.DropTable(
                name: "HouseholdEnvironments");

            migrationBuilder.DropTable(
                name: "ImagingDetails");

            migrationBuilder.DropTable(
                name: "IndividualHealthBehaviors");

            migrationBuilder.DropTable(
                name: "Insurance");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "MedicalCosts");

            migrationBuilder.DropTable(
                name: "MedicalHistories");

            migrationBuilder.DropTable(
                name: "MedicationRecords");

            migrationBuilder.DropTable(
                name: "Memos");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "PatientStaffRelations");

            migrationBuilder.DropTable(
                name: "PhysicalExaminations");

            migrationBuilder.DropTable(
                name: "ProteinDatas");

            migrationBuilder.DropTable(
                name: "PulmonaryDetails");

            migrationBuilder.DropTable(
                name: "QuestionnaireDatas");

            migrationBuilder.DropTable(
                name: "RegionalEnvironments");

            migrationBuilder.DropTable(
                name: "SpecimenInfos");

            migrationBuilder.DropTable(
                name: "SpecimenQualities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "InvestigatorQualifications");

            migrationBuilder.DropTable(
                name: "PatientBasicInfos");
        }
    }
}
