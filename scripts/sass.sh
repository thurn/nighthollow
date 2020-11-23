#!/bin/sh
set -e

cd `dirname "$0"`
cd ..

sass --watch ./Assets/Nighthollow/Interface/Styles/styles.scss ./Assets/Nighthollow/Interface/Styles/styles.uss
