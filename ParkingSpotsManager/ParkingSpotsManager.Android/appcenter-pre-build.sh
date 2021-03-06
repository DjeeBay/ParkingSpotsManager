#!/bin/bash

# ---------------------
# --- Configs:

ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/ParkingSpotsManager/ParkingSpotsManager.Android/Properties/AndroidManifest.xml
APIKEY=`grep value=\"API_KEY\" $ANDROID_MANIFEST_FILE | sed 's/.*value\s*=\s*\"\([^\"]*\)\".*/\1/g'`

echo " (i) Android Manifest path: $ANDROID_MANIFEST_FILE"
echo " (i) API KEY: $ANDROID_API_KEY"

if [ -z "${APIKEY}" ] ; then
  echo " [!] Could not find api key!"
  exit 1
fi
echo "API key detected: ${APIKEY}"
echo `grep value=\"API_KEY\" $ANDROID_MANIFEST_FILE`



# ---------------------
# --- Main:

# verbose / debug print commands

set -v

sed -i.bak "s/value="\"${APIKEY}\""/value="\"$ANDROID_API_KEY\""/" $ANDROID_MANIFEST_FILE

# ---- Remove backup:

rm -f $ANDROID_MANIFEST_FILE.bak

# ==> API key changed
echo `grep value=\"API_KEY\" $ANDROID_MANIFEST_FILE`
