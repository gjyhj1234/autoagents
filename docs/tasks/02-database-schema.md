# Task 02 — Database Schema

**GitHub Issue Title**: `[Task-02] Database Schema and Migrations`  
**Labels**: `agent-task`, `database`  
**Priority**: 🔴 Critical  
**Depends On**: #1

---

## Objective

Create the complete PostgreSQL database schema with EF Core entity definitions, migrations, and seed data (CDT procedure codes).

---

## Database Schema

All tables must have: `id UUID DEFAULT gen_random_uuid() PRIMARY KEY`, `created_at TIMESTAMPTZ DEFAULT NOW()`, `updated_at TIMESTAMPTZ DEFAULT NOW()`, `deleted_at TIMESTAMPTZ NULL` (soft delete).

### Core Tables

```sql
-- Users & Staff
CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  username VARCHAR(50) UNIQUE NOT NULL,
  email VARCHAR(255) UNIQUE NOT NULL,
  password_hash TEXT NOT NULL,
  role VARCHAR(20) NOT NULL CHECK (role IN ('admin','dentist','hygienist','front_desk','patient')),
  is_active BOOLEAN DEFAULT true,
  failed_login_attempts INT DEFAULT 0,
  locked_until TIMESTAMPTZ,
  last_login TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Providers (clinical staff)
CREATE TABLE providers (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id),
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  license_number VARCHAR(50),
  specialty VARCHAR(100),
  color VARCHAR(7) DEFAULT '#3182CE',  -- hex color for calendar
  abbreviation VARCHAR(10),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Procedure Codes (CDT/ADA codes)
CREATE TABLE procedure_codes (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  ada_code VARCHAR(10) UNIQUE NOT NULL,  -- e.g., D2391
  description TEXT NOT NULL,
  abbreviated_desc VARCHAR(50),
  category VARCHAR(100),  -- e.g., "Restorative", "Preventive"
  default_fee DECIMAL(10,2),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Patients
CREATE TABLE patients (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_number SERIAL,  -- human-readable ID
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  preferred_name VARCHAR(100),
  date_of_birth DATE NOT NULL,
  gender VARCHAR(20) CHECK (gender IN ('male','female','other','prefer_not_to_say')),
  phone_home VARCHAR(20),
  phone_mobile VARCHAR(20),
  phone_work VARCHAR(20),
  email VARCHAR(255),
  address_line1 VARCHAR(255),
  address_line2 VARCHAR(255),
  city VARCHAR(100),
  state VARCHAR(50),
  postal_code VARCHAR(20),
  country VARCHAR(50) DEFAULT 'CN',
  ssn_last4 VARCHAR(4),  -- last 4 digits only
  preferred_provider_id UUID REFERENCES providers(id),
  preferred_language VARCHAR(10) DEFAULT 'zh-CN',
  status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active','inactive','deceased','non_patient')),
  referral_source VARCHAR(100),
  notes TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Medical History
CREATE TABLE medical_histories (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  -- Common medical flags
  has_diabetes BOOLEAN DEFAULT false,
  has_hypertension BOOLEAN DEFAULT false,
  has_heart_disease BOOLEAN DEFAULT false,
  has_artificial_heart_valve BOOLEAN DEFAULT false,
  has_pacemaker BOOLEAN DEFAULT false,
  has_blood_thinners BOOLEAN DEFAULT false,
  has_bisphosphonates BOOLEAN DEFAULT false,
  has_bleeding_disorder BOOLEAN DEFAULT false,
  has_hiv BOOLEAN DEFAULT false,
  has_hepatitis BOOLEAN DEFAULT false,
  has_epilepsy BOOLEAN DEFAULT false,
  has_asthma BOOLEAN DEFAULT false,
  is_pregnant BOOLEAN DEFAULT false,
  is_nursing BOOLEAN DEFAULT false,
  -- Free-text
  other_conditions TEXT,
  current_medications TEXT,
  allergies JSONB DEFAULT '[]',  -- [{name, severity, reaction}]
  alert_flags JSONB DEFAULT '[]',  -- [{text, severity}]
  last_updated TIMESTAMPTZ DEFAULT NOW(),
  updated_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Insurance Plans
CREATE TABLE insurance_plans (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id),
  priority INT DEFAULT 1 CHECK (priority IN (1,2)),  -- 1=primary, 2=secondary
  carrier_name VARCHAR(200) NOT NULL,
  plan_name VARCHAR(200),
  group_number VARCHAR(50),
  member_id VARCHAR(50) NOT NULL,
  employer_name VARCHAR(200),
  subscriber_name VARCHAR(200),
  subscriber_dob DATE,
  subscriber_relationship VARCHAR(50),
  effective_date DATE,
  termination_date DATE,
  annual_maximum DECIMAL(10,2),
  deductible DECIMAL(10,2),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Dental Charts
CREATE TABLE dental_charts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  dentition_mode VARCHAR(20) DEFAULT 'permanent' CHECK (dentition_mode IN ('permanent','primary','mixed')),
  notation_system VARCHAR(20) DEFAULT 'universal' CHECK (notation_system IN ('universal','fdi','palmer')),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Tooth Conditions
CREATE TABLE tooth_conditions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  chart_id UUID NOT NULL REFERENCES dental_charts(id) ON DELETE CASCADE,
  tooth_number VARCHAR(5) NOT NULL,  -- "1"-"32" or "A"-"T" or FDI "11"-"48"
  surfaces VARCHAR(10)[],  -- e.g., ['M','O','D'] or ['F'] or []
  condition_type VARCHAR(50) NOT NULL,  -- e.g., 'decay','crown','bridge_pontic','missing'
  status VARCHAR(30) NOT NULL DEFAULT 'existing_other'
    CHECK (status IN ('existing_other','existing_this_office','treatment_planned',
                      'in_progress','completed','referred','watch')),
  procedure_code_id UUID REFERENCES procedure_codes(id),
  provider_id UUID REFERENCES providers(id),
  fee DECIMAL(10,2),
  note TEXT,
  date_completed DATE,
  created_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Treatment Plans
CREATE TABLE treatment_plans (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id),
  name VARCHAR(200) NOT NULL DEFAULT 'Treatment Plan',
  status VARCHAR(20) DEFAULT 'active'
    CHECK (status IN ('active','inactive','completed','rejected')),
  accepted_date DATE,
  accepted_signature TEXT,
  notes TEXT,
  created_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Treatment Plan Items
CREATE TABLE treatment_plan_items (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  plan_id UUID NOT NULL REFERENCES treatment_plans(id) ON DELETE CASCADE,
  tooth_number VARCHAR(5),
  surfaces VARCHAR(10)[],
  procedure_code_id UUID NOT NULL REFERENCES procedure_codes(id),
  status VARCHAR(30) DEFAULT 'planned'
    CHECK (status IN ('planned','scheduled','in_progress','completed','rejected')),
  provider_id UUID REFERENCES providers(id),
  fee DECIMAL(10,2),
  insurance_estimate DECIMAL(10,2),
  patient_portion DECIMAL(10,2),
  visit_number INT DEFAULT 1,
  sort_order INT DEFAULT 0,
  note TEXT,
  appointment_id UUID,  -- FK added later when appointments table exists
  date_completed DATE,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Periodontal Charts
CREATE TABLE perio_charts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id),
  exam_date DATE NOT NULL DEFAULT CURRENT_DATE,
  provider_id UUID REFERENCES providers(id),
  notes TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Periodontal Measurements (one row per tooth)
CREATE TABLE perio_measurements (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  chart_id UUID NOT NULL REFERENCES perio_charts(id) ON DELETE CASCADE,
  tooth_number VARCHAR(5) NOT NULL,
  -- Pocket depths: buccal (DB, B, MB) and lingual (DL, L, ML)
  pd_buccal_db SMALLINT, pd_buccal_b SMALLINT, pd_buccal_mb SMALLINT,
  pd_lingual_dl SMALLINT, pd_lingual_l SMALLINT, pd_lingual_ml SMALLINT,
  -- Recession (positive = recession, negative = hyperplasia)
  rec_buccal_db SMALLINT DEFAULT 0, rec_buccal_b SMALLINT DEFAULT 0, rec_buccal_mb SMALLINT DEFAULT 0,
  rec_lingual_dl SMALLINT DEFAULT 0, rec_lingual_l SMALLINT DEFAULT 0, rec_lingual_ml SMALLINT DEFAULT 0,
  -- Bleeding on Probing
  bop_buccal_db BOOLEAN DEFAULT false, bop_buccal_b BOOLEAN DEFAULT false, bop_buccal_mb BOOLEAN DEFAULT false,
  bop_lingual_dl BOOLEAN DEFAULT false, bop_lingual_l BOOLEAN DEFAULT false, bop_lingual_ml BOOLEAN DEFAULT false,
  -- Suppuration
  sup_buccal_db BOOLEAN DEFAULT false, sup_buccal_b BOOLEAN DEFAULT false, sup_buccal_mb BOOLEAN DEFAULT false,
  sup_lingual_dl BOOLEAN DEFAULT false, sup_lingual_l BOOLEAN DEFAULT false, sup_lingual_ml BOOLEAN DEFAULT false,
  -- Furcation involvement
  furcation_buccal VARCHAR(5),  -- 'I', 'II', 'III'
  furcation_lingual VARCHAR(5),
  furcation_mesial VARCHAR(5),   -- for molars with 3 roots
  -- Mobility
  mobility SMALLINT DEFAULT 0 CHECK (mobility BETWEEN 0 AND 3),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE(chart_id, tooth_number)
);

-- Operatories (chairs/rooms)
CREATE TABLE operatories (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(100) NOT NULL,
  abbreviation VARCHAR(10),
  color VARCHAR(7) DEFAULT '#718096',
  is_active BOOLEAN DEFAULT true,
  sort_order INT DEFAULT 0,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Appointment Types
CREATE TABLE appointment_types (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(100) NOT NULL,
  abbreviation VARCHAR(10),
  default_duration_minutes INT DEFAULT 60,
  color VARCHAR(7) DEFAULT '#3182CE',
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Appointments
CREATE TABLE appointments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id),
  provider_id UUID NOT NULL REFERENCES providers(id),
  operatory_id UUID REFERENCES operatories(id),
  appointment_type_id UUID REFERENCES appointment_types(id),
  start_time TIMESTAMPTZ NOT NULL,
  end_time TIMESTAMPTZ NOT NULL,
  status VARCHAR(20) DEFAULT 'scheduled'
    CHECK (status IN ('scheduled','confirmed','arrived','in_chair','completed','no_show','cancelled')),
  notes TEXT,
  created_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Audit Log
CREATE TABLE audit_logs (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id),
  action VARCHAR(20) NOT NULL CHECK (action IN ('create','update','delete','login','logout')),
  entity_type VARCHAR(100) NOT NULL,
  entity_id UUID,
  old_value JSONB,
  new_value JSONB,
  ip_address INET,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_patients_name ON patients(last_name, first_name) WHERE deleted_at IS NULL;
CREATE INDEX idx_patients_dob ON patients(date_of_birth) WHERE deleted_at IS NULL;
CREATE INDEX idx_appointments_start ON appointments(start_time) WHERE deleted_at IS NULL;
CREATE INDEX idx_appointments_provider ON appointments(provider_id, start_time) WHERE deleted_at IS NULL;
CREATE INDEX idx_tooth_conditions_chart ON tooth_conditions(chart_id) WHERE deleted_at IS NULL;
CREATE INDEX idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
```

### EF Core Entities

Create C# entity classes in `DentalChart.Core/Entities/` matching all tables above. Use:
- `public record` or `public class` — choose class for mutable entities
- Navigation properties for all foreign keys
- `[JsonIgnore]` on circular navigation properties
- Fluent API configuration in `DentalChart.Infrastructure/Data/Configurations/`

### Seed Data

`database/seeds/001_procedure_codes.sql` must include **at minimum** these CDT categories with ~10 codes each:
- Diagnostic (D0100–D0999): D0120, D0140, D0150, D0210, D0220, D0230, D0272, D0274, D0330, D0470
- Preventive (D1000–D1999): D1110, D1120, D1206, D1208, D1310, D1320, D1351, D1510
- Restorative (D2000–D2999): D2140, D2150, D2160, D2161, D2391, D2392, D2393, D2394, D2710, D2712, D2720, D2721, D2722, D2740, D2750, D2751, D2752
- Endodontic (D3000–D3999): D3310, D3320, D3330, D3346, D3347, D3348, D3410, D3421, D3425, D3426
- Periodontic (D4000–D4999): D4210, D4211, D4240, D4241, D4260, D4261, D4341, D4342, D4346, D4355
- Oral Surgery (D7000–D7999): D7140, D7210, D7220, D7230, D7240, D7241, D7250, D7310, D7311
- Prosthodontic Removable (D5000–D5899): D5110, D5120, D5130, D5140, D5211, D5212
- Prosthodontic Fixed (D6000–D6199): D6010, D6051, D6052, D6053, D6054, D6056, D6057, D6058, D6059, D6065, D6066, D6067, D6068

---

## Acceptance Criteria

- [ ] All tables created via `database/migrations/001_initial_schema.sql`
- [ ] Migration runs automatically when PostgreSQL container starts (via `initdb.d/`)
- [ ] Seed data loaded: at least 50 CDT procedure codes
- [ ] All EF Core entity classes created in `DentalChart.Core`
- [ ] EF Core `DentalChartDbContext` configured in `DentalChart.Infrastructure`
- [ ] `dotnet ef migrations add InitialCreate` produces a migration that matches the SQL schema
- [ ] Unit tests verify: entity relationships load correctly, seed data queryable
