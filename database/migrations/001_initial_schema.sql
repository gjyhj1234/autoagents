-- Enable UUID generation
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Users & Staff
CREATE TABLE IF NOT EXISTS users (
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
CREATE TABLE IF NOT EXISTS providers (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id),
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  license_number VARCHAR(50),
  specialty VARCHAR(100),
  color VARCHAR(7) DEFAULT '#3182CE',
  abbreviation VARCHAR(10),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Procedure Codes (CDT/ADA codes)
CREATE TABLE IF NOT EXISTS procedure_codes (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  ada_code VARCHAR(10) UNIQUE NOT NULL,
  description TEXT NOT NULL,
  abbreviated_desc VARCHAR(50),
  category VARCHAR(100),
  default_fee DECIMAL(10,2),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Patients
CREATE TABLE IF NOT EXISTS patients (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_number SERIAL,
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
  ssn_last4 VARCHAR(4),
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
CREATE TABLE IF NOT EXISTS medical_histories (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
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
  other_conditions TEXT,
  current_medications TEXT,
  allergies JSONB DEFAULT '[]',
  alert_flags JSONB DEFAULT '[]',
  last_updated TIMESTAMPTZ DEFAULT NOW(),
  updated_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Insurance Plans
CREATE TABLE IF NOT EXISTS insurance_plans (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id),
  priority INT DEFAULT 1 CHECK (priority IN (1,2)),
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
CREATE TABLE IF NOT EXISTS dental_charts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  dentition_mode VARCHAR(20) DEFAULT 'permanent' CHECK (dentition_mode IN ('permanent','primary','mixed')),
  notation_system VARCHAR(20) DEFAULT 'universal' CHECK (notation_system IN ('universal','fdi','palmer')),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Tooth Conditions
CREATE TABLE IF NOT EXISTS tooth_conditions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  chart_id UUID NOT NULL REFERENCES dental_charts(id) ON DELETE CASCADE,
  tooth_number VARCHAR(5) NOT NULL,
  surfaces VARCHAR(10)[],
  condition_type VARCHAR(50) NOT NULL,
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
CREATE TABLE IF NOT EXISTS treatment_plans (
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
CREATE TABLE IF NOT EXISTS treatment_plan_items (
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
  appointment_id UUID,
  date_completed DATE,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

-- Periodontal Charts
CREATE TABLE IF NOT EXISTS perio_charts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id),
  exam_date DATE NOT NULL DEFAULT CURRENT_DATE,
  provider_id UUID REFERENCES providers(id),
  notes TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Periodontal Measurements
CREATE TABLE IF NOT EXISTS perio_measurements (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  chart_id UUID NOT NULL REFERENCES perio_charts(id) ON DELETE CASCADE,
  tooth_number VARCHAR(5) NOT NULL,
  pd_buccal_db SMALLINT, pd_buccal_b SMALLINT, pd_buccal_mb SMALLINT,
  pd_lingual_dl SMALLINT, pd_lingual_l SMALLINT, pd_lingual_ml SMALLINT,
  rec_buccal_db SMALLINT DEFAULT 0, rec_buccal_b SMALLINT DEFAULT 0, rec_buccal_mb SMALLINT DEFAULT 0,
  rec_lingual_dl SMALLINT DEFAULT 0, rec_lingual_l SMALLINT DEFAULT 0, rec_lingual_ml SMALLINT DEFAULT 0,
  bop_buccal_db BOOLEAN DEFAULT false, bop_buccal_b BOOLEAN DEFAULT false, bop_buccal_mb BOOLEAN DEFAULT false,
  bop_lingual_dl BOOLEAN DEFAULT false, bop_lingual_l BOOLEAN DEFAULT false, bop_lingual_ml BOOLEAN DEFAULT false,
  sup_buccal_db BOOLEAN DEFAULT false, sup_buccal_b BOOLEAN DEFAULT false, sup_buccal_mb BOOLEAN DEFAULT false,
  sup_lingual_dl BOOLEAN DEFAULT false, sup_lingual_l BOOLEAN DEFAULT false, sup_lingual_ml BOOLEAN DEFAULT false,
  furcation_buccal VARCHAR(5),
  furcation_lingual VARCHAR(5),
  furcation_mesial VARCHAR(5),
  mobility SMALLINT DEFAULT 0 CHECK (mobility BETWEEN 0 AND 3),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE(chart_id, tooth_number)
);

-- Operatories (chairs/rooms)
CREATE TABLE IF NOT EXISTS operatories (
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
CREATE TABLE IF NOT EXISTS appointment_types (
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
CREATE TABLE IF NOT EXISTS appointments (
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
CREATE TABLE IF NOT EXISTS audit_logs (
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
CREATE INDEX IF NOT EXISTS idx_patients_name ON patients(last_name, first_name) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_patients_dob ON patients(date_of_birth) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_appointments_start ON appointments(start_time) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_appointments_provider ON appointments(provider_id, start_time) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_tooth_conditions_chart ON tooth_conditions(chart_id) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
