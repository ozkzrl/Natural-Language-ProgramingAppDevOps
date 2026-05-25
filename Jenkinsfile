pipeline {
    agent any

    stages {
        stage('1. Kodu Çek (Checkout)') {
            steps {
                // GitHub'daki en güncel kodları Rocky Linux iş alanına indirir
                checkout scm
            }
        }

        stage('2. Altyapıyı Docker Compose ile Dağıt (Deploy)') {
            steps {
                script {
                    echo 'Docker Compose mimarisi tetikleniyor...'
                    echo 'Eski konteynerler temizleniyor, imajlar yeniden derleniyor...'
                    
                    // --build: C# kodlarındaki değişiklikleri algılayıp imajı günceller
                    // -d: Konteynerleri arka planda (detached) çalıştırır
                    sh "docker compose up --build -d"
                }
            }
        }
    }

    post {
        success {
            echo 'Mükemmel! Tüm mimari Docker Compose ile başarıyla ayağa kaldırıldı.'
        }
        failure {
            echo 'Eyvah! Bir şeyler ters gitti. Lütfen Console Output ekranını inceleyin.'
        }
    }
}