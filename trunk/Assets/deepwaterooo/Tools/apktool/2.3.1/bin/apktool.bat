#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

exec java  -jar $DIR/../libexec/apktool_2.2.1.jar "$@"
