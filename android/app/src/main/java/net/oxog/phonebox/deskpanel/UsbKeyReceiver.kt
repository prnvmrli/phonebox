package net.oxog.phonebox.deskpanel

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent

/**
 * Receives the key for the USB tunnel from the agent ("adb shell am broadcast ... --es token <key>").
 * Every app on the phone can reach the tunnel, so the agent asks for this key; the manifest requires the DUMP
 * permission to send here, which adb's shell holds and ordinary apps cannot get.
 */
class UsbKeyReceiver : BroadcastReceiver() {
    override fun onReceive(context: Context, intent: Intent) {
        val key = intent.getStringExtra("token")?.takeIf { it.isNotBlank() } ?: return
        context.getSharedPreferences(AgentLink.USB_PREFS, Context.MODE_PRIVATE).edit().putString("token", key).apply()
    }
}
