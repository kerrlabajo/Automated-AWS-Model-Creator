#!/bin/bash
set -e

# Path to the lock file and commit file
LOCKFILE="/tmp/mylockfile"
COMMITFILE="/tmp/mycommitfile"

# Check if the lock file exists
if [ -e "${LOCKFILE}" ]; then
  # The lock file exists so exit the script
  echo "Another instance is running. Exiting."
  exit 1
else
  # The lock file doesn't exist so create it and proceed with the script
  touch "${LOCKFILE}"
  # Set a trap to remove the lock file when the script exits
  trap 'rm -f "${LOCKFILE}"; exit' INT TERM EXIT
fi

# Change to the directory containing your repository
cd /home/ubuntu/LSC-Trainer/docker/scripts

# Load environment variables from .env file
if [ -f .env ]
then
  export $(cat .env | sed 's/#.*//g' | xargs)
else
  echo ".env file not found"
  exit 1
fi

# Get the latest commit hash
LATEST_COMMIT=$(git rev-parse origin/feat/docker-enhancement)

# Check if the commit file exists and contains the latest commit hash
if [ -e "${COMMITFILE}" ] && grep -Fxq "${LATEST_COMMIT}" "${COMMITFILE}"; then
  # The commit file exists and contains the latest commit hash, so exit the script
  echo "There are no new commits. Exiting."
  exit 1
fi

# Check if the specific files have changes
git pull
CHANGED_FILES=$(git diff --name-only HEAD~1 HEAD)
DIRECTORY="docker/yolov5-training/"
FILE_CHANGED=0

while IFS= read -r FILE; do
  if [[ "${FILE}" == "${DIRECTORY}"* ]]; then
    echo "A file/s has been changed/added."
    FILE_CHANGED=1
  fi
done <<< "${CHANGED_FILES}"

if [ "${FILE_CHANGED}" -eq 0 ]; then
  echo "No files in ${DIRECTORY} have changed. Exiting."
  exit 1
fi

# Get the latest tag of the image
PREV_TAG=$(sudo docker images | grep "${DOCKER_IMAGE}" | awk '{print $2}' | sort -r | head -n 1)

# Get the version number and the base part of the tag
VERSION=$(echo $PREV_TAG | awk -F- '{print $1}')
TAG_BASE=$(echo $PREV_TAG | awk -F- '{print "-"$2"-"$3}')

# Increment the version by 0.1
VERSION=$(echo "$VERSION + 0.1" | bc)

# Format the version with one decimal place
VERSION=$(printf "%.1f" $VERSION)

# Create the new tag
NEW_TAG="${VERSION}${TAG_BASE}"

# Authenticate Docker to ECR
aws ecr get-login-password --region ${AWS_REGION} | sudo docker login --username AWS --password-stdin ${ECR_URL}

# Build and push the image
sudo docker build -t ${ECR_URL}/${DOCKER_IMAGE}:${NEW_TAG} -f ../yolov5-training/Dockerfile ../yolov5-training
sudo docker push ${ECR_URL}/${DOCKER_IMAGE}:${NEW_TAG}

# Remove the previous image
sudo docker rmi ${ECR_URL}/${DOCKER_IMAGE}:${PREV_TAG}

# Write the latest commit hash to the commit file
echo "${LATEST_COMMIT}" > "${COMMITFILE}"

# Remove the trap and the lock file
trap - INT TERM EXIT
rm "${LOCKFILE}"