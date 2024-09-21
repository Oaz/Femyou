snap install cmake
rm -rf bin*
cmake -S Reference-FMUs -D FMI_VERSION=2 -B bin2 && pushd bin2 && make && popd
cmake -S Reference-FMUs -D FMI_VERSION=3 -B bin3 && pushd bin3 && make && popd
