using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Labirent
{
    public partial class MainForm : Form
    {
        private int[,] labirent;
        private int[,] ziyaretSayisi;
        private int pikachuEnerji;
        private (int, int) baslangic;
        private (int, int) cikis;
        private (int, int) pikachuKonum;
        private const string labirentDosyasi = @"C:\Users\Furkan\Desktop\Algoritmalar Final proje\Labirent.txt";
        private const string sonucDosyasi = @"C:\Users\Furkan\Desktop\Algoritmalar Final proje\sonuc.txt";
        private const string pikachuPngDosyasi = @"C:\Users\Furkan\Desktop\Algoritmalar Final proje\pikachu.png";
        private const string enerjiArtisPngDosyasi = @"C:\Users\Furkan\Desktop\Algoritmalar Final proje\+.png";
        private const string enerjiAzalisPngDosyasi = @"C:\Users\Furkan\Desktop\Algoritmalar Final proje\-.png";
        private const string ashPngDosyasi = @"C:\Users\Furkan\Desktop\Algoritmalar Final proje\ash.png";
        private Bitmap pikachuGoruntu;
        private Bitmap enerjiArtisGoruntu;
        private Bitmap enerjiAzalisGoruntu;
        private Bitmap ashGoruntu;
        private int hucreBoyutu = 30;
        private bool oyunDevamEdiyor;
        private HashSet<(int, int)> enerjiTuketilenHucreler = new HashSet<(int, int)>();
        private Stack<(int, int)> kararNoktalari = new Stack<(int, int)>(); 
        private List<(int, int, string)> cozumYolu = new List<(int, int, string)>();
        private bool cikisBulundu = false; 
        private List<List<(int, int)>> tumYollar = new List<List<(int, int)>>();
        public MainForm()
        {
            InitializeComponent();
            labirentPanel.Paint += LabirentPanel_Paint;

            try
            {
                pikachuGoruntu = new Bitmap(pikachuPngDosyasi);
                enerjiArtisGoruntu = new Bitmap(enerjiArtisPngDosyasi);
                enerjiAzalisGoruntu = new Bitmap(enerjiAzalisPngDosyasi);
                ashGoruntu = new Bitmap(ashPngDosyasi);

                ResetGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            oyunDevamEdiyor = true;

            bool sonuc = cikisYoluBul(baslangic.Item1, baslangic.Item2);

            if (oyunDevamEdiyor && sonuc)
            {
                MessageBox.Show("Pikachu çıkışı buldu!");
            }
            else if (oyunDevamEdiyor)
            {
                MessageBox.Show("Pikachu çıkışı bulamadı!");
            }
        }

        private void ResetGame()
        {
            try
            {
                labirentOku(); 
                baslangic = baslangicBul();
                cikis = cikisBul();
                pikachuEnerji = 50;
                pikachuKonum = baslangic;
                ziyaretSayisi = new int[labirent.GetLength(0), labirent.GetLength(1)];
                enerjiTuketilenHucreler.Clear();
                kararNoktalari.Clear(); 
                labirentPanel.Invalidate(); 
                labirentPanel.Update();
                oyunDevamEdiyor = true;
                if (File.Exists(sonucDosyasi))
                {
                    File.WriteAllText(sonucDosyasi, string.Empty); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }

        private void labirentOku()
        {
            if (!File.Exists(labirentDosyasi))
            {
                throw new Exception("Labirent dosyası bulunamadı!");
            }

            string[] lines = File.ReadAllLines(labirentDosyasi);

            if (lines.Length == 0)
            {
                throw new Exception("Labirent dosyası boş!");
            }

            int rows = lines.Length;
            int cols = lines[0].Split(',').Length;

            labirent = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] row = lines[i].Split(',');

                if (row.Length != cols)
                {
                    throw new Exception($"Satır {i + 1} sütun sayısı hatalı!");
                }

                for (int j = 0; j < cols; j++)
                {
                    labirent[i, j] = int.Parse(row[j]);
                }
            }
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private (int, int) baslangicBul()
        {
            for (int i = 0; i < labirent.GetLength(0); i++)
            {
                for (int j = 0; j < labirent.GetLength(1); j++)
                {
                    if (labirent[i, j] == 2)
                        return (i, j);
                }
            }
            throw new Exception("Başlangıç noktası bulunamadı!");
        }

        private (int, int) cikisBul()
        {
            for (int i = 0; i < labirent.GetLength(0); i++)
            {
                for (int j = 0; j < labirent.GetLength(1); j++)
                {
                    if (labirent[i, j] == 3)
                        return (i, j);
                }
            }
            throw new Exception("Çıkış noktası bulunamadı!");
        }
        private bool cikisYoluBul(int x, int y, string yon = "")
        {
            if (!oyunDevamEdiyor) return false; 

            if (pikachuEnerji <= 0)
            {
                BaslangicaDon(); 
                return false;
            }

            if (x < 0 || y < 0 || x >= labirent.GetLength(0) || y >= labirent.GetLength(1)) return false;
            if (labirent[x, y] == 0) return false; 

            if (x == cikis.Item1 && y == cikis.Item2) return true;

            if (ziyaretSayisi[x, y] > 0)
            {
                return false;
            }

            pikachuEnerji--;

            KonumuVeEnerjiyiDosyayaYaz(yon, x, y, pikachuEnerji);

            ziyaretSayisi[x, y]++;

            if (!enerjiTuketilenHucreler.Contains((x, y)))
            {
                if (labirent[x, y] == 4) 
                {
                    pikachuEnerji += 15;
                    enerjiTuketilenHucreler.Add((x, y));
                }
                else if (labirent[x, y] == 5) 
                {
                    pikachuEnerji -= 10;
                    enerjiTuketilenHucreler.Add((x, y));
                }
            }

            PikachuHareketEttir(x, y);

            List<(int dx, int dy, string yon, int ziyaret)> yonler = new List<(int, int, string, int)>
{
    (0, 1, "Sağ", ziyaretSayisi[Clamp(x, 0, labirent.GetLength(0) - 1), Clamp(y + 1, 0, labirent.GetLength(1) - 1)]),
    (1, 0, "Aşağı", ziyaretSayisi[Clamp(x + 1, 0, labirent.GetLength(0) - 1), Clamp(y, 0, labirent.GetLength(1) - 1)]),
    (0, -1, "Sol", ziyaretSayisi[Clamp(x, 0, labirent.GetLength(0) - 1), Clamp(y - 1, 0, labirent.GetLength(1) - 1)]),
    (-1, 0, "Yukarı", ziyaretSayisi[Clamp(x - 1, 0, labirent.GetLength(0) - 1), Clamp(y, 0, labirent.GetLength(1) - 1)])
};


            yonler = yonler
      .OrderBy(yonItem => yonItem.ziyaret)
      .ThenBy(_ => Guid.NewGuid())
      .ToList();

            kararNoktalari.Push((x, y));

            foreach (var (dx, dy, newYon, _) in yonler)
            {
                int yeniX = x + dx;
                int yeniY = y + dy;

                if (cikisYoluBul(yeniX, yeniY, newYon))
                    return true;
            }

            kararNoktalari.Pop(); 

            if (kararNoktalari.Count > 0)
            {
                var oncekiKararNoktasi = kararNoktalari.Peek();
                GeriDon(oncekiKararNoktasi.Item1, oncekiKararNoktasi.Item2);
            }

            return false;
        }


        private void GeriDon(int hedefX, int hedefY)
        {
            while (pikachuKonum.Item1 != hedefX || pikachuKonum.Item2 != hedefY)
            {
                int x = pikachuKonum.Item1;
                int y = pikachuKonum.Item2;

                if (pikachuKonum.Item1 < hedefX) x++;
                else if (pikachuKonum.Item1 > hedefX) x--;

                if (pikachuKonum.Item2 < hedefY) y++;
                else if (pikachuKonum.Item2 > hedefY) y--;

                pikachuEnerji--;
                KonumuVeEnerjiyiDosyayaYaz("Geri", x, y, pikachuEnerji);

                PikachuHareketEttir(x, y);
            }
        }
        private void BaslangicaDon()
        {
            oyunDevamEdiyor = false;

            ResetGame();

            cikisYoluBul(baslangic.Item1, baslangic.Item2);
        }


        private void KonumuVeEnerjiyiDosyayaYaz(string yon, int x, int y, int enerji)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(sonucDosyasi, true))
                {
                    string satir = $"{yon}: ({x}, {y}) - Enerji: {enerji}";
                    sw.WriteLine(satir);
                    Console.WriteLine(satir); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosyaya yazarken hata oluştu: {ex.Message}");
            }
        }


        private void PikachuHareketEttir(int x, int y)
        {
            if (!oyunDevamEdiyor) return;

            var (eskiX, eskiY) = pikachuKonum; 
            pikachuKonum = (x, y);
            Graphics g = labirentPanel.CreateGraphics();

            g.FillRectangle(new SolidBrush(Color.White),
                eskiY * hucreBoyutu, eskiX * hucreBoyutu, hucreBoyutu, hucreBoyutu);
            g.DrawRectangle(Pens.Black,
                eskiY * hucreBoyutu, eskiX * hucreBoyutu, hucreBoyutu, hucreBoyutu);

            g.DrawImage(pikachuGoruntu, y * hucreBoyutu, x * hucreBoyutu, hucreBoyutu, hucreBoyutu);

            g.Dispose();

            System.Threading.Thread.Sleep(80);
        }

        private void LabirentPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < labirent.GetLength(0); i++)
            {
                for (int j = 0; j < labirent.GetLength(1); j++)
                {
                    if (labirent[i, j] == 0)
                        g.FillRectangle(new SolidBrush(Color.Black), j * hucreBoyutu, i * hucreBoyutu, hucreBoyutu, hucreBoyutu);
                    else if (labirent[i, j] == 4)
                        g.DrawImage(enerjiArtisGoruntu, j * hucreBoyutu, i * hucreBoyutu, hucreBoyutu, hucreBoyutu);
                    else if (labirent[i, j] == 5)
                        g.DrawImage(enerjiAzalisGoruntu, j * hucreBoyutu, i * hucreBoyutu, hucreBoyutu, hucreBoyutu);

                    g.DrawRectangle(Pens.Black, j * hucreBoyutu, i * hucreBoyutu, hucreBoyutu, hucreBoyutu);
                }
            }

            int ashWidth = hucreBoyutu * 2;
            int ashHeight = hucreBoyutu * 2;
            g.DrawImage(ashGoruntu,
                (labirent.GetLength(1) - 2 + 2) * hucreBoyutu,
                (labirent.GetLength(0) - 2 - 2) * hucreBoyutu,
                ashWidth, ashHeight);

            var (pikachuX, pikachuY) = pikachuKonum;
            g.DrawImage(pikachuGoruntu, pikachuY * hucreBoyutu, pikachuX * hucreBoyutu, hucreBoyutu, hucreBoyutu);
        }
    }
}
