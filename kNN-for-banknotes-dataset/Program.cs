using System;
using System.IO;

namespace kNN_for_banknotes_dataset
{
    class Program
    {
        /*
         * Hem kNN algoritmasında hemde banknotları yazdırmada kullanmak için 4 özelliği alarak
         * öklid uzaklığı ile özellikleri girilen banknota en yakın k tane banknot değerinin özelliklerini
         * ve uzaklığını döndüren method tanımlandı. Parametre olarak k değeri, sınıflandırılacak banknot ve
         * banknot veri seti alındı.
         */
        public static double[,] kNoktalarıBulma(int k, double[] banknot, double[,] banknotVeriSeti)
        {
            double[] kDistanceArray = new double[banknotVeriSeti.GetLength(0)];//veri setindeki banknotların, sınıflandırılacak banknota uzaklığını tutan dizi oluşturuldu

            for (int i = 0; i < banknotVeriSeti.GetLength(0); i++)//for döngüsüyle verisetindeki bütün değerler ile sınıflandırılacak banknot arasındaki uzaklıklar bulundu
            {
                double distance = Math.Pow(Math.Pow(banknotVeriSeti[i, 0] - banknot[0], 2) + Math.Pow(banknotVeriSeti[i, 1] - banknot[1], 2) +
                    Math.Pow(banknotVeriSeti[i, 2] - banknot[2], 2) + Math.Pow(banknotVeriSeti[i, 3] - banknot[3], 2), 0.5);
                kDistanceArray[i] = distance; // i indexli banknot verisi ile sınıflandırılacak banknot arasındaki uzaklık diziye eklendi
            }

            double[,] kDegerleri = new double[k, 6];//bulunan k banknotlarının özelliklerini ve sınıflandırılan banknota olan uzaklığını içeren dizi oluşturuldu
            int kIndex = 0; // kDegerleri matrisine sırasıyla özelliklerin atılması için index sayacı alındı
            for (int i = 0; i < k; i++)//k tane yakın banknotun teker teker bulunması için k kere dönen for döngüsü alındı
            {
                int banknot_index = 0;//en yakın noktayı belirleyen index tutucu değişken alındı
                for (int j = 0; j < kDistanceArray.Length - 1; j++)
                {
                    //Eger verisetindeki ilk banknot k değeri çıkarsa aynı değeri tekrardan vermemesi için kontrol edilecek index 1 öne atılıyor
                    if (kDistanceArray[banknot_index] < 0)
                    {
                        banknot_index = j + 1;
                    }
                    //En yakın noktaların veri setindeki indexi bulunması için gereken koşullar verildi if yapısında test edildi
                    if (kDistanceArray[banknot_index] > kDistanceArray[j + 1] && kDistanceArray[j + 1] >= 0)
                    {
                        banknot_index = j + 1;
                    }
                }
                for (int t = 0; t < banknotVeriSeti.GetLength(1); t++)
                {
                    kDegerleri[kIndex, t] = banknotVeriSeti[banknot_index, t];//k indexi bulunduktan sonra veri setindeki özellikleri kDegerleri iki boyutlu dizisine sırasıyla aktarılır 
                }
                kDegerleri[kIndex, 5] = kDistanceArray[banknot_index]; //Banknot sınıflandırmada uzaklığıda yazdırmamız istendiği için döndürülecek dizinin içine banknot ile arasındaki uzaklıkta eklendi
                kDistanceArray[banknot_index] = -1;//Uzaklık negatif olmayacağından eğer k değeri bulunduysa tekrar seçilmemesi için değer uzaklık dizisinde negatif yapılır

                kIndex++;//bulunan k değeri sayacla arttırıldı
            }
            return kDegerleri; //k tane en yakın banknotun özellikleri ve sınıflandırılacak banknot ile arasındaki uzaklıklarının olduğu dizi döndürülür
        }
        public static int kNNSınıflandırma(int k, double[] nokta, double[,] banknotVeriSeti)//Verilen banknotun hangi sınıftan olduğunu bulan kNN Sınıflandırma methodu alındı
        {
            double[,] kDegerleri = kNoktalarıBulma(k, nokta, banknotVeriSeti);//Önceden yazdığımız k noktalarını bulan method çagırılır ve kDegerleri adlı diziye banknotlar eklenir

            int sahte_count = 0;//k tane banknottan 0 sınıfından olanları saymak için sayaç
            int gercek_count = 0;//k tane banknottan 1 sınıfından olanları saymak için sayaç
            for (int i = 0; i < kDegerleri.GetLength(0); i++)//k tane banknotun sınıfları sayılır
            {
                if (kDegerleri[i, 4] == 0)
                {
                    sahte_count++;
                }
                else if (kDegerleri[i, 4] == 1)
                {
                    gercek_count++;
                }
            }

            //k değerlerinin sınıfları sayıldıktan sonra hangi sınıf daha fazla çıktıysa döndürülücek değişkene atanır
            if (gercek_count < sahte_count)
            {
                return 0;//Eğer k değerlerinin çogu 0 sınıfındansa knn algoritması 0 döndürür
            }
            else if (gercek_count > sahte_count)
            {
                return 1; //Eğer k değerlerinin çogu 1 sınıfındansa knn algoritması 1 döndürür
            }
            else
            {
                return Convert.ToInt32(kDegerleri[0, 4]);//Eğer 0 ve 1 sınıfları eşit çıkarsa ilk k değerinin sınıfı döndürülür
            }
        }

        public static void banknotSiniflandirma(int k, double[] banknot, double[,] banknotVeriSeti)
        {
            double[,] kDegerleri = kNoktalarıBulma(k, banknot, banknotVeriSeti);//k değerlerini bulan methodumuzu çagırdık
            //Tablo düzgün bir şekilde oluşturmaya çalıştık ve k değerleri tablonun düzgün gözükmesi için formatlanarak yazdırdık
            Console.WriteLine("  k       Varyans Değeri       Çarpıklık Değeri         Basıklık Değeri         Entropi Değeri        Nokta ile Olan Uzaklığı           Türü");
            Console.WriteLine("-------|-------------------|-----------------------|-----------------------|----------------------|-------------------------------|-------------");
            int kSayisi = 1;
            for (int i = 0; i < kDegerleri.GetLength(0); i++)
            {
                Console.WriteLine("{0,4} {1,15} {2,20} {3,20} {4,25} {5,30} {6,20}",
                   kSayisi++, kDegerleri[i, 0].ToString(), kDegerleri[i, 1], kDegerleri[i, 2], kDegerleri[i, 3], kDegerleri[i, 5], kDegerleri[i, 4]);

            }

            //Paranın gerçekmi,sahtemi olduğunu knn sınıflandırma algoritmamızı çağırarak ölçüp yazdırdık
            if (kNNSınıflandırma(k, banknot, banknotVeriSeti) == 1)
            {
                Console.WriteLine("\nBanknot Türü 1(Gerçek)\n");
            }
            else
            {
                Console.WriteLine("\nBanknot Türü 0(Sahte)\n");
            }
        }

        public static void basariOlcumu(int k, double[,] banknotVeriSeti)//banknot veri setinin k değerine göre başarı ölçümü yapan method
        {
            double[,] testSeti = new double[200, 5];//test verisi
            double[,] kalanVeriSeti = new double[1172, 5];//test verisini çıkarınca kalan veri
            int gercekParaSayaci = 0;//ana banknot veri setindeki 1 sınıflı son 100  veriyi test verisine eklemek için alınan sayaç
            int sahteParaSayaci = 0;//ana banknot veri setindeki 0 sınıflı son 100  veriyi test verisine eklemek için alınan sayaç
            int testSetineEklenenElemanSayisi = 0;//test verisindeki indexi ayarlamak için alınan sayaç
            int kalanVeriSetineEklenenElemanSayisi = 0;//geri kalan veri setindeki elemanların indexini ayarlamak için alınan sayaç

            //Test ve Train Veri Seti oluşturma

            for (int i = banknotVeriSeti.GetLength(0) - 1; i > 0; i--)
            {
                if (banknotVeriSeti[i, 4] == 1 && gercekParaSayaci < 100)//1 sınıflı son 100 veriyi test verisine ekleyen kontrol mekanizması
                {
                    for (int j = 0; j < banknotVeriSeti.GetLength(1); j++)//özellikleri sırasıyla ekleyen döngü
                    {
                        testSeti[testSetineEklenenElemanSayisi, j] = banknotVeriSeti[i, j];
                    }
                    gercekParaSayaci++;
                    testSetineEklenenElemanSayisi++;
                }
                else if (banknotVeriSeti[i, 4] == 0 && sahteParaSayaci < 100)//0 sınıflı son 100 veriyi test verisine ekleyen kontrol mekanizması
                {
                    for (int j = 0; j < banknotVeriSeti.GetLength(1); j++)//özellikleri sırasıyla ekleyen döngü
                    {
                        testSeti[testSetineEklenenElemanSayisi, j] = banknotVeriSeti[i, j];
                    }
                    sahteParaSayaci++;
                    testSetineEklenenElemanSayisi++;
                }
                else//Geriye kalan 1172 veriyi bir diziye eklemek için oluşturulan kontrol mekanizması
                {
                    for (int j = 0; j < banknotVeriSeti.GetLength(1); j++)//özellikleri sırasıyla ekleyen döngü
                    {
                        kalanVeriSeti[kalanVeriSetineEklenenElemanSayisi, j] = banknotVeriSeti[i, j];
                    }
                    kalanVeriSetineEklenenElemanSayisi++;
                }
            }

            int dogruTahmin = 0;//doğru sınıflandırılan banknotları sayan sayaç
            //Banknot Başarı Testi
            for (int i = 0; i < testSeti.GetLength(0); i++)//test setindeki banknotları kalan 1172 veride test eden döngü
            {
                //test setindeki indexte bulunan özellikler bir diziye aktarılır
                double[] testBanknot = { testSeti[i, 0], testSeti[i, 1], testSeti[i, 2], testSeti[i, 3], testSeti[i, 4] };
                int banknotTahmin = kNNSınıflandırma(k, testBanknot, kalanVeriSeti);//a şıkkındaki kNN sınıflandırma ile banknot tahmin edilir
                banknotSiniflandirma(k, testBanknot, kalanVeriSeti);//b şıkkındaki her bir k değerinin özelliklerini yazdıran method çagırıldı
                Console.WriteLine("Gerçek Değer: {0} , Tahmin Edilen Değer: {1}", testBanknot[4], banknotTahmin);//Gerçek değer ve Tahmin Edilen Değer yazdırıldı
                if (banknotTahmin == testBanknot[4])//Eğer knn algoritmasıyla ölçtüğümüz doğruysa sayaç arttırılır
                {
                    dogruTahmin++;
                }
            }
            //dogru tahmin edilen değerler ile gerçek değerler bölünüp 100 ile çarpılarak başarı yüzdesi alınıp yazdırılır
            double basariOrani = (Convert.ToDouble(dogruTahmin) / Convert.ToDouble(testSeti.GetLength(0))) * 100;
            Console.WriteLine("\nk={0} değerinde Başarı Oranı: %{1:0.0}\n", k, basariOrani);
        }


        public static void VeriSetiListeleme(double[,] banknotVeriSeti)
        {
            //Txt deki banknot veri setindeki bütün veriler yazdırılır
            Console.WriteLine("  Ornek No      Varyans Değeri     Çarpıklık Değeri      Basıklık Değeri         Entropi Değeri      Türü");
            Console.WriteLine("--------------|-----------------|---------------------|-----------------------|------------------|----------");
            for (int i = 0; i < banknotVeriSeti.GetLength(0); i++)
            {
                Console.WriteLine("{0,4} {1,22} {2,16} {3,22} {4,22} {5,15}",
                                    i + 1, banknotVeriSeti[i, 0], banknotVeriSeti[i, 1], banknotVeriSeti[i, 2], banknotVeriSeti[i, 3], banknotVeriSeti[i, 4]);
            }
        }
        static void Main(string[] args)
        {
            try
            {
                double[,] banknotVeriSeti = new double[1372, 5]; //txt dosyasındaki 1372 tane banknot verisi için iki boyutlu double dizisi tanımlandı
                //Dosya okuma için gerekli kodlar yazıldı
                FileStream fs = new FileStream("data_banknote_authentication.txt", FileMode.Open, FileAccess.Read);
                StreamReader sw = new StreamReader(fs);
                string satir = sw.ReadLine();//Dosyadaki ilk satır okundu
                int index = 0;//verisetinde gerekli indexi yerleştirmek için bir sayaç alındı
                while (satir != null) //txt dosyasının satırlar bitene kadar okunmasını sağlayan döngüyü oluşturduk
                {
                    string[] banknot = satir.Split(",");//satırdaki değerler arasındaki virgüller sayesinde değerler ayrıştırılarak bir dizi içerisine alındı
                    for (int i = 0; i < banknot.Length; i++)
                    {
                        //değerler stringden double'ye convert edilebilmesi için string manipülasyonu kullanarak noktalar virgüllere çevrildi
                        banknot[i] = banknot[i].Replace(".", ",");
                        double ozellik = Convert.ToDouble(banknot[i]);//özellikler teker teker gerekli index'e yerleştirilmiştir
                        banknotVeriSeti[index, i] = ozellik;
                    }
                    index++;//sayaç arttırıldı
                    satir = sw.ReadLine();//yeni satır okundu
                }
                //Banknot bilgileri ve k değerini kullanıcıdan aldırma işlemi
                Console.Write("Lütfen k değerini giriniz: ");
                int k = Convert.ToInt32(Console.ReadLine());
                Console.Write("Banknot'un Varyans Değerini Giriniz: ");
                string varyans = Console.ReadLine();

                Console.Write("Banknot'un Çarpıklık Değerini Giriniz: ");
                string carpiklik = Console.ReadLine();

                Console.Write("Banknot'un Basıklık Değerini Giriniz: ");
                string basiklik = Console.ReadLine();

                Console.Write("Banknot'un Entropi Değerini Giriniz: ");
                string entropi = Console.ReadLine();
                //Oluşan banknot bilgileri bir dizi içerisine sırasıyla alındı
                double[] banknotBilgileri = { Double.Parse(varyans), Double.Parse(carpiklik), Double.Parse(basiklik), Double.Parse(entropi) };

                banknotSiniflandirma(k, banknotBilgileri, banknotVeriSeti);//banknot sınıflandırma methodu çağırıldı

                Console.Write("Başarı Ölçümü yapmak istediğiniz k değerini giriniz:");//Başarı ölçümü yapılacak k değeri kullanıcıdan alınır
                k = Convert.ToInt32(Console.ReadLine());
                basariOlcumu(k, banknotVeriSeti);//verilen k değerine göre başarı ölçümü yapan ve yüzdesini ekrana yazdıran method çağırıldı

                Console.WriteLine("Veri Listelemeye Geçmek İçin Herhangi Bir Tuşa Basınız...");
                Console.ReadKey();

                VeriSetiListeleme(banknotVeriSeti);//txt den okunan banknot veri setini ekrana listeleyen methodu çagırdık
            }
            catch (Exception e)
            {
                Console.WriteLine(e);//Eğer bir hata yakalanırsa konsola yazdırma işlemi
            }
        }
    }
}

