#include "Tree.h"

CTree::CTree(void)
{
	this->mBegin = 0;
	this->mEnd = 0;
	this->mGasStation = NULL;
	this->mGasCount = 0;
	this->mAdjacencyMatrix = NULL;
	this->mLineCount = 0;

	this->mHeadNode = NULL;
	this->mCurrentNode = NULL;
	this->mListMinimumLoad.clear();
	this->mListPassNode.clear();
	this->mCurrentDepth = 0;
}

CTree::~CTree(void)
{
}

void CTree::InitWithTxt()
{
	ifstream fin;
	fin.open( "input.txt" );

	//ó���� �� ���� ��� id ���ϱ�
	fin >> this->mBegin; //���� ���� id
	fin >> this->mEnd; //���� ���� id

	//������ ����
	fin >> this->mGasCount; //������ ����
	this->mGasStation = new int[this->mGasCount];

	int id=0;
	for( int i=0; i<this->mGasCount; i++ )
	{
		fin >> id; //������ idrmrj
		if( id == i) //�������� ������ id �Է°�����, ������ �´����� ����
			fin >> this->mGasStation[i];
		else
		{
			cout << "input ���� ���� ���� " << endl;
			exit(1);
		}
	}

	//�� �������� ��� ����ġ(����)�� �����ϱ� ���Ͽ� ������� �����
	fin >> this->mLineCount; //�� ����
	
	this->mAdjacencyMatrix = new int*[this->mLineCount];
	for( int i=0; i<this->mLineCount; i++)
	{
		this->mAdjacencyMatrix[i] = new int[this->mLineCount];
	}

	for( int i=0; i<this->mLineCount; i++ )
	{
		for( int j=0; j<this->mLineCount; j++ )
		{
			this->mAdjacencyMatrix[i][j] = -1; //�ڱ� �ڽ��̰ų� , ���� ���� �濡�� -1��
		}
	}

	int row = 0, col = 0; //��, ��
	int dis = 0; //�Ÿ�
	for( int i=0; i<this->mLineCount; i++)
	{
		fin >> row;
		fin >> col;
		fin >> dis;

		this->mAdjacencyMatrix[row][col] = dis;
	}

	fin.close();
}

void CTree::DisplayMatrix()
{
	for( int i=0; i<this->mLineCount; i++ )
	{
		for( int j=0; j<this->mLineCount; j++ )
		{
			cout  << setw(5) << this->mAdjacencyMatrix[i][j];
		}
		cout << endl;
	}
}

void CTree::Start()
{
	int id = 0;
	while( id != mEnd )
	{
		if( this->mHeadNode == NULL) //ó���� ��尡 ���� ��
		{
			Insert( mBegin ); //ó�� ��� ����
			id = this->mQueueWorkLoad.front();
			this->mQueueWorkLoad.pop();
		}
		//ù�� ° ����( ���� ��� �α��� ����ġ�� 0�̱� ���� )
		//����ġ : �������� �Ÿ� * ���� ������ �⸧��
		//ex) 0->1 - 9km, 10km �⸧�� ������ �ֱ� ������ 9km�� �׳� �� �� �ִ� => ����ġ 0
		//ex) 1->2 - 5km, 5km�� ���� ���ؼ� �߰��� 4km �ʿ� => ����ġ : 4km * 1�� ������ �⸧��
		else 
		{
			Insert(id);
			id = this->mQueueWorkLoad.front();
			this->mQueueWorkLoad.pop();
		}
	}
}

void CTree::Insert(int id)
{
	CNode* temp = NULL;
	if( this->mHeadNode == NULL )//Head�� ����� ��, ���� ù ���
	{
		temp = new CNode();
		temp->mID = id; //id
		temp->mPrice = this->mGasStation[id];

		temp->mTotalPrice = 0; //ó���̴ϱ� ����ġ�� 0
		temp->mDepth = 0; //ó�� ����
		temp->mOil = 10; //ó�� �����ҿ��� ������ �ִ� �⸧�� 10L
		temp->mSpendOil = 0;

		temp->mPreNode = NULL; //ó�� ����̴ϱ� �ڿ� ���� ����.

		this->mHeadNode = temp; //���� ���� ��� �Ҵ�
		mListPassNode.push_back(temp); //������ ������ ����.
		mListMinimumLoad.push_back(id); //�ּ��� �濡 ����.
	}
	else
	{
		for( this->mIterNode=this->mListPassNode.begin(); mIterNode!=this->mListPassNode.end(); this->mIterNode++)
		{
			if( (*this->mIterNode)->mID == id )
				break;
		}
		temp = *this->mIterNode; //���� ó���ؾ��� ��� ��������
	}
	int distance = 0;
	//������ �ѹ�
	for( int i=0; i<this->mGasCount; i++ )
	{
		distance = this->mAdjacencyMatrix[id][i];
		if( distance != -1 && distance != id )
		{
			ApproachNodeInsert(distance, temp, i);
		}
	}
	distance = 0;
	//������ �� �ѹ�
	for( int i=0; i<this->mGasCount; i++ )
	{
		distance = this->mAdjacencyMatrix[i][id];if( distance != -1 && distance != id )
		{
			ApproachNodeInsert(distance, temp, i);
		}
	}
}

void CTree::ApproachNodeInsert(int distance, CNode* preNode, int nowID)
{
	CNode* apTemp = new CNode();
	apTemp->mID = nowID;
	apTemp->mPrice = this->mGasStation[nowID]; 
	if( preNode->mOil - distance >= 0 ) //�����ҷ� ���� �濡 �⸧�� ���Ұų� �� ���� ��
	{
		apTemp->mTotalPrice = preNode->mTotalPrice; //�⸧�� ���ұ� ������ �⸧�� �ȵ���
		apTemp->mSpendOil = 0;
		apTemp->mOil = preNode->mOil - distance; //���� �����ҿ��� ���� ������ �Ÿ���ŭ ���������
	}
	else //�⸧���� ����� ��
	{
		apTemp->mSpendOil =  -(preNode->mOil-distance); //�����ҿ��� �⸧ ä�� ��
		apTemp->mTotalPrice = preNode->mTotalPrice + apTemp->mSpendOil * preNode->mPrice; //���ڶ� �⸧ * ���� ������ �⸧��
		apTemp->mOil = 0; //���� �����ҿ��� ���� �����Ҹ�ŭ�� �⸧�� ������ �־����� ����� 0
	}
	apTemp->mDepth = preNode->mDepth+1; //���� ������ ���� ���� +1
	apTemp->mPreNode = preNode; //���� ���� ���� �����Ҹ� ����Ų��.
	
	for( this->mIterNode=this->mListPassNode.begin(); mIterNode!=this->mListPassNode.end(); this->mIterNode++)
	{
		if( (*this->mIterNode)->mID == nowID )
			break;
	}
	
	if( this->mIterNode != this->mListPassNode.end() ) //������ ó���ߴ� node���
	{
		if( (*this->mIterNode)->mTotalPrice > apTemp->mTotalPrice ) //���İ��� ��� ����ġ�� �� ũ�� �ٲ۴�.
		{
			CNode* removeNode = *this->mIterNode;
			removeNode->mPreNode->mListApproachNode.remove(removeNode); //���� ����� �������� �߿��� �ش� ��� ����

			delete removeNode;
			removeNode = NULL;

			this->mListPassNode.remove( *this->mIterNode );

			//������ node�� id�� �����ϱ� ���Ͽ� �ٽ� ť�� �����....
			queue<int> temp;
			while( !this->mQueueWorkLoad.empty() )
			{
				int id = this->mQueueWorkLoad.front();
				this->mQueueWorkLoad.pop();
				if( id == nowID )
				{
					continue;
				}
				else if( !mExtraApproachNode.empty() )
				{
					list<int>::iterator iter;
					for( iter = mExtraApproachNode.begin(); iter!=mExtraApproachNode.end(); iter++ )
					{
						for( this->mIterNode=this->mListPassNode.begin(); mIterNode!=this->mListPassNode.end(); this->mIterNode++)
						{
							if( (*this->mIterNode)->mID == *iter )
							{
								this->mListPassNode.remove(*this->mIterNode);
								break;
							}
						}

					}
					continue;
				}
				else
					temp.push( id );
			}
			this->mQueueWorkLoad = temp;
		}
		else
		{ //���� ��尡 �� �ּҺ���̶�� �ƹ��͵� ���ϰ� ��
			delete apTemp;
			apTemp = NULL;
			return ;
		}
	}
	preNode->mListApproachNode.push_back(apTemp); //���� ����� ���� ��忡 �߰�

	this->mQueueWorkLoad.push(apTemp->mID); //���� ���� ������ ó���ؾ� �� ���
	this->mListPassNode.push_back(apTemp);//�ѹ��̶�� ��ģ ��带 ����
	//this->mListPassNode[apTemp->mID] = apTemp; 
}

