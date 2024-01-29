source .env

# TODO: Create a script in yolov5 folder to generalize and call `train.py` or `export.py` with parameters.
# TODO: After executing train and export, move the file according to the parameters destination.
# TODO: Set `build_and_push.sh`` to receive parameters instead of environment variables.

# Retrieve from developer/user's .env file
ACCOUNT_ID=${ACCOUNT_ID}
REGION=${REGION}
REPO_NAME=${REPO_NAME}
IMAGE_TAG=${IMAGE_TAG}

# Increment the image tag
IMAGE_TAG=$((IMAGE_TAG + 1))
echo "export IMAGE_TAG=$IMAGE_TAG" >> .env
IMAGE_TAG=${IMAGE_TAG}

docker build -f ../docker/Dockerfile -t $REPO_NAME ../docker

docker tag $REPO_NAME $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPO_NAME:$IMAGE_TAG

aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com

aws ecr describe-repositories --repository-names $REPO_NAME > /dev/null 2>&1 || aws ecr create-repository --repository-name $REPO_NAME > /dev/null 2>&1

docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPO_NAME:$IMAGE_TAG