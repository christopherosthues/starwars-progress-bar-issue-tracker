package com.christopherosthues.starwarsprogressbarissuetracker

import androidx.compose.ui.window.Window
import androidx.compose.ui.window.application
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.createDataStore
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.dataStoreFileName

fun main() {
    val appName = "IssueTracker"
    val prefs = createDataStore {
        val osName = System.getProperty("os.name").lowercase()
        val directory = when {
            osName.contains("win") -> "${System.getenv("APPDATA")}\\$appName\\"
            osName.contains("mac") -> "${System.getProperty("user.home")}/Library/Application Support/$appName/"
            osName.contains("nix") || osName.contains("nux") || osName.contains("aix") -> "${System.getProperty("user.home")}/.local/share/$appName/"
            else -> "${System.getProperty("user.home")}/"
        }
        directory + dataStoreFileName
    }
    application {
        Window(
            onCloseRequest = ::exitApplication,
            title = "starwars-progress-bar-issue-tracker-app",
        ) {
            App(
                prefs = prefs
            )
        }
    }
}