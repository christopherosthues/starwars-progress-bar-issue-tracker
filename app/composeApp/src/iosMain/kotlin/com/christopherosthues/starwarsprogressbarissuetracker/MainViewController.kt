package com.christopherosthues.starwarsprogressbarissuetracker

import androidx.compose.runtime.remember
import androidx.compose.ui.window.ComposeUIViewController
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.createDataStore

fun MainViewController() = ComposeUIViewController {
    App(
        prefs = remember { createDataStore() }
    )
}
