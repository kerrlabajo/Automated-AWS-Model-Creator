source .env

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

aws ecr describe-repositories --repository-names $REPO_NAME || aws ecr create-repository --repository-name $REPO_NAME

docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPO_NAME:$IMAGE_TAG