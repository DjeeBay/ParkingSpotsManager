#!/bin/bash

# ---------------------
# --- Configs:

echo " (i) Provided Android Manifest path: $ANDROID_MANIFEST_FILE"
echo " (i) API KEY: $ANDROID_API_KEY"

ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/Droid/Properties/AndroidManifest.xml
APIKEY=`grep value=\"API_KEY\" $ANDROID_MANIFEST_FILE | sed 's/.*value\s*=\s*\"\([^\"]*\)\".*/\1/g'`

if [ -z "${APIKEY}" ] ; then
  echo " [!] Could not find api key!"
  exit 1
fi
echo "API key detected: ${APIKEY}"



# ---------------------
# --- Main:

# verbose / debug print commands

set -v

sed -i.bak "s/value="\"${APIKEY}\""/value="\"$ANDROID_API_KEY\""/" $ANDROID_MANIFEST_FILE

# ---- Remove backup:

rm -f $ANDROID_MANIFEST_FILE.bak

# ==> API key changed
