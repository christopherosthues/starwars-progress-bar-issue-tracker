import org.jetbrains.compose.desktop.application.dsl.TargetFormat
import org.jetbrains.kotlin.gradle.ExperimentalKotlinGradlePluginApi
import org.jetbrains.kotlin.gradle.ExperimentalWasmDsl
import org.jetbrains.kotlin.gradle.dsl.JvmTarget
import org.jetbrains.kotlin.gradle.targets.js.webpack.KotlinWebpackConfig
//import org.jlleitschuh.gradle.ktlint.reporter.ReporterType

fun properties(key: String) = providers.gradleProperty(key)

plugins {
    alias(libs.plugins.kotlinMultiplatform)
    alias(libs.plugins.androidApplication)
    alias(libs.plugins.composeMultiplatform)
    alias(libs.plugins.composeCompiler)
    alias(libs.plugins.changelog)
    alias(libs.plugins.detekt)
    alias(libs.plugins.kotlinxSerialization)
    alias(libs.plugins.kover)
//    alias(libs.plugins.ktlint)
    alias(libs.plugins.apollographql)
    alias(libs.plugins.sonarqube)
}

kotlin {
    @OptIn(ExperimentalWasmDsl::class)
    wasmJs {
        moduleName = "composeApp"
        browser {
            val rootDirPath = project.rootDir.path
            val projectDirPath = project.projectDir.path
            commonWebpackConfig {
                outputFileName = "composeApp.js"
                devServer = (devServer ?: KotlinWebpackConfig.DevServer()).apply {
                    static = (static ?: mutableListOf()).apply {
                        // Serve sources to debug inside browser
                        add(rootDirPath)
                        add(projectDirPath)
                    }
                }
            }
        }
        binaries.executable()
    }
    
    androidTarget {
        @OptIn(ExperimentalKotlinGradlePluginApi::class)
        compilerOptions {
            jvmTarget.set(JvmTarget.JVM_17)
        }
    }
    
    jvm("desktop")
    
    listOf(
        iosX64(),
        iosArm64(),
        iosSimulatorArm64()
    ).forEach { iosTarget ->
        iosTarget.binaries.framework {
            baseName = "ComposeApp"
            isStatic = true
        }
    }
    
    sourceSets {
        val desktopMain by getting
        
        androidMain.dependencies {
            implementation(compose.preview)
            implementation(libs.androidx.activity.compose)
            implementation(libs.kotlinx.coroutines.android)
            implementation(libs.ktor.client.okhttp)
        }
        commonMain.dependencies {
            implementation(compose.runtime)
            implementation(compose.foundation)
            implementation(compose.material3)
            implementation(compose.ui)
            implementation(compose.components.resources)
            implementation(compose.components.uiToolingPreview)
            implementation(libs.androidx.lifecycle.viewmodel)
            implementation(libs.androidx.lifecycle.runtime.compose)
            implementation(libs.apollographql.client)
            implementation(libs.kotlinx.coroutines.core)
            implementation(libs.kotlinx.serialization)
//            implementation(libs.ktor.client.auth)
            implementation(libs.ktor.client.cio)
//            implementation(libs.ktor.client.content.negotiation)
            implementation(libs.ktor.client.core)
//            implementation(libs.ktor.client.logging)
            implementation(libs.ktor.serialization.kotlinx.json)
//            implementation("ch.qos.logback:logback-classic:$logback_version")
        }
        desktopMain.dependencies {
            implementation(compose.desktop.currentOs)
            implementation(libs.kotlinx.coroutines.swing)
        }
        iosMain.dependencies {
            implementation(libs.ktor.client.darwin)
        }
    }
}

detekt {
    toolVersion = libs.versions.detekt.get()
    config.from("${rootDir}/config/detekt/detekt.yml")
    buildUponDefaultConfig = true
}

//ktlint {
//    filter {
//        exclude("**/generated/**")
//    }
//    verbose = true
//    version = libs.versions.ktlint.get()
//    outputToConsole = true
//    coloredOutput = true
//    reporters {
//        reporter(ReporterType.CHECKSTYLE)
//        reporter(ReporterType.JSON)
//        reporter(ReporterType.HTML)
//    }
//}

changelog {
    groups = listOf("Added", "Changed", "Removed", "Fixed")
    repositoryUrl = properties("pluginRepositoryUrl")
    keepUnreleasedSection = true
}

kover {
    reports {
        total {
            xml {
                onCheck = true
            }
        }
    }
}

apollo {
    service("service") {
        packageName.set("com.christopherosthues.starwarsprogressbarissuetracker.graphql")
    }
}

tasks.withType<io.gitlab.arturbosch.detekt.Detekt>().configureEach {
    reports {
        html.required = true
        html.outputLocation = file("build/reports/mydetekt.html")
        md.required = false
        sarif.required = false
        txt.required = false
        xml.required = false
    }
}

tasks.withType<io.gitlab.arturbosch.detekt.Detekt>().configureEach {
    this.jvmTarget = "17"
}
tasks.withType<io.gitlab.arturbosch.detekt.DetektCreateBaselineTask>().configureEach {
    this.jvmTarget = "17"
}

sonar {
    properties {
        property("sonar.projectKey", "christopherosthues_starwars-progress-bar-issue-tracker-server")
        property("sonar.organization", "christopherosthues")
        property("sonar.host.url", "https://sonarcloud.io")
    }
}

android {
    namespace = "com.christopherosthues.starwarsprogressbarissuetracker"
    compileSdk = libs.versions.android.compileSdk.get().toInt()

//    sourceSets["main"].manifest.srcFile("src/androidMain/AndroidManifest.xml")
//    sourceSets["main"].res.srcDirs("src/androidMain/res")
//    sourceSets["main"].resources.srcDirs("src/commonMain/resources")

    defaultConfig {
        applicationId = "com.christopherosthues.starwarsprogressbarissuetracker"
        minSdk = libs.versions.android.minSdk.get().toInt()
        targetSdk = libs.versions.android.targetSdk.get().toInt()
        versionCode = 1
        versionName = "1.0"
    }
    packaging {
        resources {
            excludes += "/META-INF/{AL2.0,LGPL2.1}"
        }
    }
    buildTypes {
        getByName("release") {
            isMinifyEnabled = false
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
//    buildFeatures {
//        compose = true
//    }
//    dependencies {
//        debugImplementation(compose.uiTooling)
//    }
}

compose.desktop {
    application {
        mainClass = "com.christopherosthues.starwarsprogressbarissuetracker.MainKt"

        nativeDistributions {
            targetFormats(TargetFormat.Dmg, TargetFormat.Msi, TargetFormat.Deb)
            packageName = "com.christopherosthues.starwarsprogressbarissuetracker"
            packageVersion = "1.0.0"
        }
    }
}
