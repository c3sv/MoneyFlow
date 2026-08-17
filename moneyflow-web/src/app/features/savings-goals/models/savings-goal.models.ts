export interface SavingsGoal {
  id: number;
  title: string;
  targetAmount: number;
  currentAmount: number;
  deadline: string | null;
}

export interface CreateSavingsGoalRequest {
  title: string;
  targetAmount: number;
  deadline: string | null;
}

export interface CreateSavingsGoalResponse {
  id: number;
}

export interface UpdateSavingsGoalRequest {
  title: string;
  targetAmount: number;
  deadline: string | null;
}

export interface AddSavingsGoalProgressRequest {
  amount: number;
}
