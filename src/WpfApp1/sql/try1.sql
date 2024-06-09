-- SQLite compatible script

-- Table structure for table `biodata`

DROP TABLE IF EXISTS biodata;
CREATE TABLE biodata (
  NIK TEXT NOT NULL PRIMARY KEY,
  nama TEXT DEFAULT NULL,
  tempat_lahir TEXT DEFAULT NULL,
  tanggal_lahir DATE DEFAULT NULL,
  jenis_kelamin TEXT DEFAULT NULL,
  golongan_darah TEXT DEFAULT NULL,
  alamat TEXT DEFAULT NULL,
  agama TEXT DEFAULT NULL,
  status_perkawinan TEXT DEFAULT NULL,
  pekerjaan TEXT DEFAULT NULL,
  kewarganegaraan TEXT DEFAULT NULL
);

-- Table structure for table `sidik_jari`

DROP TABLE IF EXISTS sidik_jari;
CREATE TABLE sidik_jari (
  berkas_citra TEXT,
  nama TEXT DEFAULT NULL
);

-- Data dumping for table `biodata` and `sidik_jari` can be added here if available

-- End of script
