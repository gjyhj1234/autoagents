INSERT INTO procedure_codes (code, description, category)
VALUES
    ('D0120', 'Periodic oral evaluation', 'diagnostic'),
    ('D1110', 'Adult prophylaxis', 'preventive'),
    ('D2391', 'Resin-based composite one surface posterior', 'restorative')
ON CONFLICT (code) DO NOTHING;
