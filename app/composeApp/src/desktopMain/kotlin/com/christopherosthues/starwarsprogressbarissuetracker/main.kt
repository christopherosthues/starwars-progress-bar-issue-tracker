package com.christopherosthues.starwarsprogressbarissuetracker

import androidx.compose.ui.window.Window
import androidx.compose.ui.window.application

fun main() = application {
    Window(
        onCloseRequest = ::exitApplication,
        title = "starwars-progress-bar-issue-tracker-app",
    ) {
        App()
    }
}