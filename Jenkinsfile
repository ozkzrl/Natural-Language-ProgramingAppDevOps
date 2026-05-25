pipeline {
    agent any

    stages {
        stage('1. Kodu Çek (Checkout)') {
            steps {
                // GitHub'dan en güncel kodları Rocky Linux'a indirir
                checkout scm
            }
        }

        stage('2. Altyapıyı Docker Compose ile Dağıt (Deploy)') {
            steps {
                script {
                    echo 'Docker Compose mimarisi tetikleniyor...'
                    echo 'Eski konteynerler indiriliyor, imajlar yeniden derleniyor...'
                    
                    // Kök dizindeki docker-compose.yml dosyasını okur, 
                    // kod değişikliklerine göre backend'i baştan derler (--build)
                    // ve tüm servisleri arka planda (-d) ayağa kaldırır.
                    sh "docker-compose up --build -d"
                }
            }
        }
    }

    post {
        success {
            echo 'Mükemmel! Docker Compose hattı başarıyla tamamladı. Tüm servisler ayakta!'
        }
        failure {
            echo 'Eyvah! Bir şeyler ters gitti. Lütfen Console Output ekranını inceleyin.'
        }
    }
}