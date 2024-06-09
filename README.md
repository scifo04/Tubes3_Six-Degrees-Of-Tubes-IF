# Tubes3_Six-Degrees-Of-Tubes-IF

Program merupakan permanfaatan dari algoritma String Matching untuk membuat Fingerprint Detector

# Algoritma

Program ini menggunakan algoritma String Matching untuk mencari kemiripan dari sidik jari inputan pengguna dengan sidik jari dalam database.

2 algoritma yang digunakan dalam String Matching:

1. Knuth Morris Pratt (KMP)
   Algoritma yang merupakan pengembangan dari brute force, algoritma ini menggunakan fungsi pinggiran sebagai mark ketika suatu indeks mengalami error ketika melakukan perbandingan, maka pergeseran akan dilakukan sesuai indeks pada fungsi pinggiran
2. Boyer Moore (BM)
   BM adalah algoritma pencocokan pola yang berdasarkan dua teknik yaitu looking-glass technique dan character-jump technique. Pada looking-glass, hal ini melibatkan pencarian P di T secara mundur melalui P dan mulai dari karakter yang paling terakhir. Pada character-jump, hal ini dilibatkan ketika terjadi ketidakcocokan dalam salah satu huruf di kata yang ingin diproses dengan kata yang akan dicari kesamaannya akan kata yang ingin di proses.

Usai melakukan string matching maka akan mendapatkan nama pemilik sidik jari, yang selanjutnya akan dicocokkan dengan databse nama alay menggunakan regex

# Requirements

- [.NET](https://dotnet.microsoft.com/id-id/download/dotnet-framework)

# Library

1. System.Data.SQLite
2. System.Drawing.Common
3. MySql.Data

# Running

1. Clone repository
   ```
   git clone https://github.com/scifo04/Tubes3_Six-Degrees-Of-Tubes-IF.git
   ```
2. Pindah direktori
   ```
   cd Tubes3_Six-Degrees-Of-Tubes-IF/src/WpfApp1/WpfApp1
   ```
3. Masukkan command
   ```
   Install-Package System.Data.SQLite
   Install-Package System.Drawing.Common
   Install-Package MySql.Data
   ```
4. Masukkan command
   ```
   dotnet run
   ```
5. Masukkan gambar yang ingin dicari sidik jarinya
6. Pilih algoritma yang ingin digunakan
7. Klik tombol _search_
8. Tunggu unutk mendapatkan hasil
9. Hasil akan muncul ketika box biodata ditekan

## Side Note

Untuk dapat melakukan input data, diperlukan sebuah file sql yang suitable dengan SQLite.

# Author

Group name: Six Degrees of Tubes IF

| Nama                        | NIM      |
| --------------------------- | -------- |
| Daniel Mulia Putra Manurung | 13522043 |
| Haikal Assyauqi             | 13522052 |
| Marvin Scifo Y Hutahaean    | 13522110 |
