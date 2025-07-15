
    using UnityEngine;
    /// Oyuncuyu ARPG (Diablo, PoE gibi) tarzında takip eden kamera sistemi.
    /// Bu script, belirlenen hedefi (genellikle oyuncu) yumuşak bir gecikmeyle (damping) takip eder.
    /// Kamera pozisyonu ve rotasyonu, sahnedeki ilk ayarına göre korunur ve sadece pozisyon güncellenir.
    public class PlayerCameraFollower : MonoBehaviour
    {
        // --- Dependencies & Fields ---

        [Header("Takip Ayarları")]
        [Tooltip("Kameranın takip edeceği hedef Transform. Genellikle oyuncu atanır.")]
        [SerializeField] private Transform _target; // Takip edilecek hedef (Oyuncu). SerializeField ile Inspector'dan atanabilir hale getirildi.

        [Tooltip("Kameranın hedeften ne kadar uzakta ve hangi açıda duracağını belirleyen ofset vektörü.")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 12f, -8f); // Hedefe göre kameranın konumsal farkı. Bu değerler ARPG açısı için iyi bir başlangıçtır.

        [Tooltip("Kameranın takip hareketinin ne kadar yumuşak olacağını belirler. Düşük değerler daha yumuşak (daha gecikmeli) takip sağlar.")]
        [Range(0.01f, 1.0f)]
        [SerializeField] private float _smoothSpeed = 0.125f; // Takip yumuşaklığı. Lerp fonksiyonunda kullanılacak.

        // --- Unity Lifecycle Methods ---

        /// <summary>
        /// LateUpdate, tüm Update ve FixedUpdate işlemleri tamamlandıktan sonra çalışır.
        /// Kamera hareketlerini burada yapmak, hedefin (oyuncunun) o frame'deki tüm hareketlerini tamamlamasından sonra
        /// kameranın güncellenmesini sağlar. Bu, takipten kaynaklanan titremeleri (jitter) önler.
        /// </summary>
        private void LateUpdate()
        {
            // Eğer bir hedef atanmamışsa, hata vermemek için fonksiyondan çık.
            // Bu, hedefin yok edilmesi gibi durumlarda NullReferenceException hatasını önler.
            if (_target == null)
            {
                // Geliştirme aşamasında bir uyarı log'u basmak faydalı olabilir.
                // GameDebug.Log("Kamera için bir hedef atanmamış!", LogType.Warning);
                return;
            }

            // 1. İstenen Pozisyonu Hesapla:
            // Hedefin mevcut pozisyonuna ofseti ekleyerek kameranın olmasını istediğimiz ideal konumu buluruz.
            Vector3 desiredPosition = _target.position + _offset;

            // 2. Pozisyonu Yumuşat:
            // Kameranın mevcut pozisyonu ile olması gereken pozisyon arasında yumuşak bir geçiş yaparız.
            // Vector3.Lerp (Linear Interpolation), iki nokta arasında belirli bir oranda bir nokta bulur.
            // _smoothSpeed değeri bu oranı kontrol eder, her frame'de aradaki mesafenin bir kısmını kat ederiz.
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);

            // 3. Kameranın Pozisyonunu Güncelle:
            // Hesaplanan yumuşatılmış pozisyonu kameranın transform'una uygularız.
            transform.position = smoothedPosition;
        }

        // --- Public Methods ---
        /// Bu kamera takipçisini yeni bir hedefle başlatır.
        /// Bu metod, bağımlılıkları dışarıdan enjekte etmek (Dependency Injection) için kullanılır.
        /// Örneğin, bir GameManager sahne yüklendiğinde oyuncuyu bulup bu metoda parametre olarak verebilir.
        /// Bu sayede bu script, spesifik olarak 'Player' objesini aramak zorunda kalmaz ve daha modüler olur.
        /// <param name="newTarget">Takip edilecek yeni Transform.</param>
        public void Initialize(Transform newTarget)
        {
            _target = newTarget;
        }
    }
