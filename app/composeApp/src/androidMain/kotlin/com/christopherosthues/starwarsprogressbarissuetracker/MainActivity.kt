package com.christopherosthues.starwarsprogressbarissuetracker

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.remember
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.createDataStore

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {
            App(
                prefs = remember { createDataStore(this) }
            )
        }
    }
}
