-- =============================================================
-- DATA: shadows
-- =============================================================
INSERT INTO public.shadows (id, code_name, clan, origin, rank) VALUES
   ('550e8400-e29b-41d4-a716-000000000178', 'Silent Dagger',     'Hyuga Clan',        'Land of Rain',      'Toshiyama'),
   ('550e8400-e29b-41d4-a716-000000000179', 'Silent Talon',      'Uzumaki Clan',      'Land of Sound',     'Oniwaban'),
   ('550e8400-e29b-41d4-a716-000000000180', 'Silent Serpent',    'Uchiha Clan',       'Land of Fire',      'Danza')
ON CONFLICT (id) DO NOTHING;

-- =============================================================
-- DATA: stealthmetrics
-- =============================================================
INSERT INTO public.stealthmetrics (id, shadow_id, shadow_blend_score, silence_rating, invisibility_duration_ms, acrobatics_level) VALUES
  ('b2c3d4e5-f6a7-4b8c-9d0e-000000000178', '550e8400-e29b-41d4-a716-000000000178',  98,  48, 4215, 'Intermediate'),
  ('b2c3d4e5-f6a7-4b8c-9d0e-000000000179', '550e8400-e29b-41d4-a716-000000000179',  89,  62, 3577, 'Beginner'),
  ('b2c3d4e5-f6a7-4b8c-9d0e-000000000180', '550e8400-e29b-41d4-a716-000000000180',  36,  92, 3932, 'Beginner')
ON CONFLICT (id) DO NOTHING;