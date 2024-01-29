# TODO: Create a script in yolov5 folder to generalize and call `train.py` or `export.py` with parameters.
# TODO: After executing train and export, move the file according to the parameters destination.
# TODO: Set `build_and_push.sh`` to receive parameters instead of environment variables.

# Retrieve from developer/user's .env file
REGISTRY_ALIAS=$1
REPO_NAME=$2
IMAGE_TAG=$3

# Check if all arguments are provided
if [ -z "$REGISTRY_ALIAS" ] || [ -z "$REPO_NAME" ] || [ -z "$IMAGE_TAG" ]; then
    echo "Usage: $0 <region> <registry_alias> <repo_name> <image_tag>"
    exit 1
fi

docker build -f ../docker/ubuntu-cuda/Dockerfile -t ubuntu-cuda ../docker/ubuntu-cuda

docker build -f ../docker/yolov5/Dockerfile -t $REPO_NAME ../docker/yolov5

docker rmi ubuntu-cuda

docker tag $REPO_NAME public.ecr.aws/$REGISTRY_ALIAS/$REPO_NAME:$IMAGE_TAG

aws ecr-public get-login-password --region us-east-1 | docker login --username AWS --password-stdin public.ecr.aws

aws ecr-public describe-repositories --repository-names $REPO_NAME > /dev/null 2>&1 || aws ecr-public create-repository --repository-name $REPO_NAME > /dev/null 2>&1

docker push public.ecr.aws/$REGISTRY_ALIAS/$REPO_NAME:$IMAGE_TAG