Get Started with Firebase Cloud Messaging for Android


Configuring Firebase
--------------------

1. Create a Firebase project in the [Firebase Console][1], if you don't already have one setup.  If you have an existing Google project associated with your mobile app, you can use the **Import Google Project** option.  Otherwise, use the **Create New Project** option.

2. Click **Add Firebase to your Android app**.  

  a. Enter your app's ***Package Name***
  
  b. Enter the SHA-1 Hash for your signing certificate.  If you don't know how to find this, [check out this guide][2]

3. Click **Add App** and download the ***google-services.json*** file generated for you.

4. Add the ***google-services.json*** file to your Xamarin.Android project.

5. Set the *Build Action* for your ***google-services.json*** file to ***GoogleServicesJson***.  

The build process will take the api keys and values from your ***google-services.json*** and translate them into the correct resource string key/value pairs in your app.







## Editing your AndroidManifest.xml

Currently, you will need to manually add the broadcast receiver declarations to your app's `AndroidManifest.xml` file.  You should add these declarations inside of your `<application ..></application>` element:

```xml
<!-- START Manual Firebase Additions -->
<receiver
  android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver"
  android:exported="false" />

<receiver
  android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver"
  android:exported="true"
  android:permission="com.google.android.c2dm.permission.SEND" >
	<intent-filter>
		<action android:name="com.google.android.c2dm.intent.RECEIVE" />
		<action android:name="com.google.android.c2dm.intent.REGISTRATION" />
		<category android:name="${applicationId}" />
	</intent-filter>
</receiver>
<!-- END Manual Firebase Additions -->
```


## Create your Service

If you want to do any messaging handling beyond receiving notifications on apps in the background, such as receiving notifications in foregrounded apps, receiving data payloads, or sending upstream messages.

The service should extend `FirebaseMessagingService`, and declare the same intent filter as in the example below:

```csharp
[Service]
[IntentFilter (new [] { "com.google.firebase.MESSAGING_EVENT" })]
public class MyFirebaseMessagingService : FirebaseMessagingService
{
    public override void OnMessageReceived (RemoteMessage message)
    {
        // TODO(developer): Handle FCM messages here.
        // If the application is in the foreground handle both data and notification messages here.
        // Also if you intend on generating your own notifications as a result of a received FCM
        // message, here is where that should be initiated. See sendNotification method below.
        Android.Util.Log.Debug (TAG, "From: " + message.From);
        Android.Util.Log.Debug (TAG, "Notification Message Body: " + message.GetNotification ().Body);
    }
}
```

You will also need a service that subclasses `FirebaseInstanceIdService` to handle the creation, rotation, and updating of registration tokens.  These tokens are required for sending notifications to specific devices, or adding devices to device groups:

```csharp
[Service]
[IntentFilter (new [] { "com.google.firebase.INSTANCE_ID_EVENT" })]
public class MyFirebaseIIDService : FirebaseInstanceIdService
{
    /**
     * Called if InstanceID token is updated. This may occur if the security of
     * the previous token had been compromised. Note that this is called when the InstanceID token
     * is initially generated so this is where you would retrieve the token.
     */
    public override void OnTokenRefresh ()
    {
        // Get updated InstanceID token.
        var refreshedToken = FirebaseInstanceId.Instance.Token;
        
        // TODO: Implement this method to send any registration to your app's servers.
    }
}
```

If FCM is critical to the Android app's function, be sure to set the minimum SDK Version to 8 or higher.  This ensures that the Android app cannot be installed in an environment in which it could not run properly.

 
### Check for Google Play Services APK

Apps that rely on the Play Services SDK should always check the device for a compatible Google Play services APK before accessing Google Play services features.  It is recommended to do this in two places: in the main activity's `OnCreate ()` method, and in its `OnResume ()` method.


The check in `OnCreate ()` ensures that the app can't be used without a successful check. The check in `OnResume ()` ensures that if the user returns to the running app through some other means, such as through the back button, the check is still performed. If the device doesn't have a compatible Google Play services APK, your app can call `GooglePlayServicesUtil.GetErrorDialog ()` to allow users to download the APK from the Google Play Store or enable it in the device's system settings.

## Access the device registration token

On initial startup of your app, the FCM SDK generates a registration token for the client app instance. If you want to target single devices or create device groups, you'll need to access this token by extending `FirebaseInstanceIdService` as in the previous example.

This section describes how to retrieve the token and how to monitor changes to the token. Because the token could be rotated after initial startup, you are strongly recommended to retrieve the latest updated registration token.

The registration token may change when:

 - The app deletes Instance ID
 - The app is restored on a new device
 - The user uninstalls/reinstall the app
 - The user clears app data.
 - Retrieve the current registration token

When you need to retrieve the current token, call `FirebaseInstanceID.Instance.Token`. This property returns null if the token has not yet been generated.


### Monitor token generation

The `OnTokenRefreshcallback` method fires whenever a new token is generated, so accessing `Token` in its context ensures that you are accessing a current, available registration token. Make sure you have added the service to your manifest, then call `Token` in the context of `OnTokenRefresh`, and log the value as shown:

```csharp
public override void OnTokenRefresh ()
{
    // Get updated InstanceID token.
    var refreshedToken = FirebaseInstanceId.Instance.Token;
    Android.Util.Log.Debug (TAG, "Refreshed token: " + refreshedToken);

    // If you want to send messages to this application instance or
    // manage this apps subscriptions on the server side, send the
    // Instance ID token to your app server.
    sendRegistrationToServer(refreshedToken);
}
```

After you've obtained the token, you can send it to your app server and store it using your preferred method. 


Samples
=======

You can find a Sample Application within each Firebase component.  The sample will demonstrate the necessary configuration and some basic API usages.






Learn More
==========

You can learn more about the various Firebase SDKs & APIs by visiting the official [Firebase][5] documentation




[1]: https://console.developers.google.com/ "Google Developers Console"
[2]: https://developer.xamarin.com/guides/android/deployment,_testing,_and_metrics/MD5_SHA1/ "Finding your SHA-1 Fingerprints"
[3]: https://developers.google.com/android/ "Google APIs for Android"
[4]: https://firebase.google.com/console/ "Firebase Developer Console"
[5]: https://firebase.google.com/ "Firebase"
