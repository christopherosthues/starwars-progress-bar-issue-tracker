//import org.jlleitschuh.gradle.ktlint.reporter.ReporterType

plugins {
    // this is necessary to avoid the plugins to be loaded multiple times
    // in each subproject's classloader
    alias(libs.plugins.androidApplication) apply false
    alias(libs.plugins.androidLibrary) apply false
    alias(libs.plugins.composeMultiplatform) apply false
    alias(libs.plugins.composeCompiler) apply false
    alias(libs.plugins.kotlinMultiplatform) apply false
    alias(libs.plugins.changelog) apply false
    alias(libs.plugins.detekt) apply false
    alias(libs.plugins.kotlinxSerialization) apply false
    alias(libs.plugins.kover) apply false
//    alias(libs.plugins.ktor) apply false
//    alias(libs.plugins.ktlint) apply false
    alias(libs.plugins.sonarqube) apply false
}
